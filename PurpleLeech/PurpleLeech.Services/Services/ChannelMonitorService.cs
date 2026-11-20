using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Avalonia.Threading;
using Newtonsoft.Json;
using PurpleLeech.Core.Enums;
using PurpleLeech.Core.Models;
using PurpleLeech.Services.Interfaces;

namespace PurpleLeech.Services.Services
{
    /// <summary>
    /// Watches channels and captures the VOD of a stream while it is live, so the download is finished
    /// moments after the stream ends and before Twitch mutes any copyrighted audio.
    /// </summary>
    internal class ChannelMonitorService : IChannelMonitorService, IDisposable
    {
        #region Constants

        private const int POLL_INTERVAL_SECONDS = 15;
        private const string SETTINGS_FILE = "monitor.json";

        private static readonly Regex LoginRegex = new Regex("^[a-zA-Z0-9_]{1,25}$");

        #endregion Constants

        #region Fields

        private readonly IApiService _apiService;
        private readonly IDownloadService _downloadService;
        private readonly IPreferencesService _preferencesService;
        private readonly IFilenameService _filenameService;
        private readonly IFolderService _folderService;
        private readonly IRuntimeDataService _runtimeDataService;
        private readonly ILogService _logService;

        private readonly Timer _timer;
        private readonly object _settingsLock = new object();
        private int _checkRunning;
        private bool _isEnabled = true;

        #endregion Fields

        #region Constructors

        public ChannelMonitorService(
            IApiService apiService,
            IDownloadService downloadService,
            IPreferencesService preferencesService,
            IFilenameService filenameService,
            IFolderService folderService,
            IRuntimeDataService runtimeDataService,
            ILogService logService)
        {
            _apiService = apiService;
            _downloadService = downloadService;
            _preferencesService = preferencesService;
            _filenameService = filenameService;
            _folderService = folderService;
            _runtimeDataService = runtimeDataService;
            _logService = logService;

            Channels = new ObservableCollection<MonitoredChannel>();

            Load();

            _timer = new Timer(_ => Check(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(POLL_INTERVAL_SECONDS));
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<MonitoredChannel> Channels { get; }

        public int PollIntervalSeconds => POLL_INTERVAL_SECONDS;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                _isEnabled = value;
                Save();

                foreach (MonitoredChannel channel in Channels.ToList())
                {
                    if (!value && !channel.IsCapturing)
                    {
                        channel.Status = "Monitoring paused";
                    }
                }

                if (value)
                {
                    CheckNow();
                }
            }
        }

        public event Action<string> CaptureStarted;

        #endregion Properties

        #region Methods

        public void AddChannel(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException(nameof(login));
            }

            login = NormalizeLogin(login);

            if (!LoginRegex.IsMatch(login))
            {
                throw new ArgumentException($"'{login}' is not a valid channel name!");
            }

            if (OnUiThread(() => Channels.Any(c => c.Login == login)))
            {
                return;
            }

            // Network call, may run on a background thread
            if (!_apiService.ChannelExists(login))
            {
                throw new ArgumentException($"The channel '{login}' does not exist!");
            }

            OnUiThread(() =>
            {
                if (!Channels.Any(c => c.Login == login))
                {
                    Channels.Add(new MonitoredChannel(login));
                }

                return true;
            });

            Save();
            CheckNow();
        }

        public void RemoveChannel(string login)
        {
            MonitoredChannel channel = Channels.FirstOrDefault(c => c.Login == NormalizeLogin(login));

            if (channel != null)
            {
                Channels.Remove(channel);
                Save();
            }
        }

        public void CheckNow()
        {
            ThreadPool.QueueUserWorkItem(_ => Check());
        }

        private static string NormalizeLogin(string login)
        {
            login = login.Trim();

            // Accept pasted channel urls like https://www.twitch.tv/somechannel
            int slash = login.TrimEnd('/').LastIndexOf('/');

            if (slash >= 0)
            {
                login = login.TrimEnd('/').Substring(slash + 1);
            }

            return login.ToLowerInvariant();
        }

        private void Check()
        {
            if (Interlocked.Exchange(ref _checkRunning, 1) == 1)
            {
                return;
            }

            try
            {
                List<MonitoredChannel> channels = OnUiThread(() => Channels.ToList());

                if (channels.Count == 0 || !_isEnabled)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(_runtimeDataService.RuntimeData.AuthInfo?.AccessToken))
                {
                    SetStatus(channels, "Log in to monitor channels");
                    return;
                }

                Dictionary<string, LiveStreamInfo> liveStreams;

                try
                {
                    liveStreams = _apiService.GetLiveStreams(channels.Select(c => c.Login));
                }
                catch (Exception ex)
                {
                    SetStatus(channels, "Check failed: " + ex.Message);
                    return;
                }

                DateTime now = DateTime.Now;

                foreach (MonitoredChannel channel in channels)
                {
                    liveStreams.TryGetValue(channel.Login, out LiveStreamInfo stream);

                    bool capturing = IsCaptureActive(channel);

                    OnUiThread(() =>
                    {
                        channel.LastChecked = now;
                        channel.IsLive = stream != null;
                        channel.IsCapturing = capturing;
                        return true;
                    });

                    if (stream == null)
                    {
                        SetStatus(channel, capturing ? "Offline, finishing capture" : "Offline");
                        continue;
                    }

                    if (stream.StreamId == channel.CapturedStreamId)
                    {
                        SetStatus(channel, capturing ? "Live, recording" : "Live, capture ended (see Downloads)");
                        continue;
                    }

                    try
                    {
                        StartCapture(channel, stream);
                    }
                    catch (Exception ex)
                    {
                        _logService.LogException(ex);
                        SetStatus(channel, "Live, capture failed: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }
            finally
            {
                Interlocked.Exchange(ref _checkRunning, 0);
            }
        }

        private void StartCapture(MonitoredChannel channel, LiveStreamInfo stream)
        {
            Vod video = _apiService.GetArchiveVideoForStream(stream.UserId, stream.StreamId);

            if (video == null)
            {
                // The VOD shows up a little after the stream starts, or never if the channel does not save VODs
                SetStatus(channel, "Live, waiting for the VOD to appear");
                return;
            }

            VodAuthInfo vodAuthInfo = _apiService.GetVodAuthInfo(video.Id);

            if (!vodAuthInfo.Privileged && vodAuthInfo.SubOnly)
            {
                SetStatus(channel, "Live, but the VOD is sub-only and you are not subscribed");
                return;
            }

            Dictionary<VodQuality, string> playlistInfo = _apiService.GetPlaylistInfo(video.Id, vodAuthInfo);
            List<VodQuality> qualities = playlistInfo.Keys.OrderBy(q => q).ToList();

            if (qualities.Count == 0)
            {
                SetStatus(channel, "Live, waiting for the VOD playlist");
                return;
            }

            Preferences prefs = _preferencesService.CurrentPreferences.Clone();

            VodQuality quality = QualitySelector.Select(qualities, prefs.DownloadDefaultQuality);

            string folder = prefs.DownloadSubfoldersForFav && _preferencesService.IsChannelInFavourites(video.Channel)
                ? Path.Combine(prefs.DownloadFolder, video.Channel)
                : prefs.DownloadFolder;

            string filename = _filenameService.SubstituteWildcards(prefs.DownloadFileName, video, quality);
            filename = _filenameService.EnsureExtension(filename, prefs.DownloadDisableConversion);
            filename = GetUnusedFilename(folder, filename);

            DownloadParameters downloadParams = new DownloadParameters(video, qualities, quality, folder, filename,
                prefs.DownloadDisableConversion)
            {
                IsLiveCapture = true,
                LiveChannelLogin = channel.Login,
                LiveStreamId = stream.StreamId
            };

            OnUiThread(() =>
            {
                _downloadService.Enqueue(downloadParams);
                channel.CapturedStreamId = stream.StreamId;
                channel.IsCapturing = true;
                channel.Status = "Live, recording";
                CaptureStarted?.Invoke(stream.UserName ?? channel.Login);
                return true;
            });
        }

        private string GetUnusedFilename(string folder, string filename)
        {
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            string candidate = filename;

            for (int i = 2; File.Exists(Path.Combine(folder, candidate)) ||
                            _downloadService.IsFileNameUsed(Path.Combine(folder, candidate)); i++)
            {
                candidate = $"{name} ({i}){ext}";
            }

            return candidate;
        }

        private bool IsCaptureActive(MonitoredChannel channel)
        {
            return OnUiThread(() => _downloadService.Downloads.Any(d =>
                d.DownloadParams.IsLiveCapture &&
                d.DownloadParams.LiveChannelLogin == channel.Login &&
                (d.DownloadState == DownloadState.Queued || d.DownloadState == DownloadState.Downloading)));
        }

        private void SetStatus(IEnumerable<MonitoredChannel> channels, string status)
        {
            foreach (MonitoredChannel channel in channels)
            {
                SetStatus(channel, status);
            }
        }

        private void SetStatus(MonitoredChannel channel, string status)
        {
            OnUiThread(() =>
            {
                channel.Status = status;
                return true;
            });
        }

        private static T OnUiThread<T>(Func<T> func)
        {
            return Dispatcher.UIThread.CheckAccess() ? func() : Dispatcher.UIThread.Invoke(func);
        }

        private string SettingsPath => Path.Combine(_folderService.GetAppDataFolder(), SETTINGS_FILE);

        private void Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    return;
                }

                MonitorSettings settings = JsonConvert.DeserializeObject<MonitorSettings>(File.ReadAllText(SettingsPath));

                if (settings == null)
                {
                    return;
                }

                _isEnabled = settings.Enabled;

                foreach (string login in settings.Channels ?? new List<string>())
                {
                    if (!string.IsNullOrWhiteSpace(login) && !Channels.Any(c => c.Login == login))
                    {
                        Channels.Add(new MonitoredChannel(login));
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }
        }

        private void Save()
        {
            try
            {
                lock (_settingsLock)
                {
                    MonitorSettings settings = new MonitorSettings
                    {
                        Enabled = _isEnabled,
                        Channels = OnUiThread(() => Channels.Select(c => c.Login).ToList())
                    };

                    Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                    File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        #endregion Methods

        private class MonitorSettings
        {
            public bool Enabled { get; set; } = true;

            public List<string> Channels { get; set; } = new List<string>();
        }
    }
}
