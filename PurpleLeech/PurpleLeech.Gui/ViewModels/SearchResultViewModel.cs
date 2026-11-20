using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using PurpleLeech.Core.Events;
using PurpleLeech.Core.Models;
using PurpleLeech.Gui.Interfaces;
using PurpleLeech.Gui.Types;
using PurpleLeech.Services.Interfaces;
using PurpleLeech.Shared.Commands;
using PurpleLeech.Shared.Events;

namespace PurpleLeech.Gui.ViewModels
{
    public partial class SearchResultViewModel : ViewModelBase, INavigationState
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISearchService _searchService;
        private readonly IFilenameService _filenameService;

        private readonly object _commandLockObject;

        private ICommand _viewCommand;
        private ICommand _downloadCommand;
        private ICommand _seachCommand;

        private bool _isAuthenticatedSubOnly;

        #endregion Fields

        #region Constructors

        public SearchResultViewModel(
            IEventAggregator eventAggregator,
            IApiService apiService,
            IAuthService authService,
            IDialogService dialogService,
            INavigationService navigationService,
            IPreferencesService preferencesService,
            ISearchService searchService,
            IFilenameService filenameService)
        {
            _eventAggregator = eventAggregator;
            _apiService = apiService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _preferencesService = preferencesService;
            _searchService = searchService;
            _filenameService = filenameService;

            _searchService.PropertyChanged += SearchService_PropertyChanged;

            _commandLockObject = new object();

            _eventAggregator.GetEvent<SubOnlyAuthChangedEvent>().Subscribe(SubOnlyAuthChanged);

            _isAuthenticatedSubOnly = authService.IsAuthenticatedSubOnly;
        }

        #endregion Constructors

        #region Properties

        public Vector ScrollPosition { get; set; }

        public ObservableCollection<Vod> Videos
        {
            get { return _searchService.Videos; }
        }

        public ICommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new DelegateCommand<string>(ViewVideo);
                }

                return _viewCommand;
            }
        }

        public ICommand DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                {
                    _downloadCommand = new DelegateCommand<string>(DownloadVideo);
                }

                return _downloadCommand;
            }
        }

        public ICommand SeachCommnad
        {
            get
            {
                if (_seachCommand == null)
                {
                    _seachCommand = new DelegateCommand(ShowSearch);
                }

                return _seachCommand;
            }
        }

        #endregion Properties

        #region Methods

        private void SubOnlyAuthChanged(bool isAuthenticatedSubOnly)
        {
            _isAuthenticatedSubOnly = isAuthenticatedSubOnly;
        }

        private void ViewVideo(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        Vod video = Videos.Where(v => v.Id == id).FirstOrDefault();

                        if (video != null && video.Url != null && video.Url.IsAbsoluteUri)
                        {
                            StartVideoStream(video);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void StartVideoStream(Vod video)
        {
            try
            {
                Preferences currentPrefs = _preferencesService.CurrentPreferences;

                if (currentPrefs.MiscUseExternalPlayer)
                {
                    Process.Start(currentPrefs.MiscExternalPlayer, video.Url.ToString());
                }
                else
                {
                    // Do not execute any string as command
                    if (!video.Url.ToString().StartsWith("https://")) {
                        throw new Exception("Unsupported video URL");
                    }
                    
                    Process.Start(new ProcessStartInfo(video.Url.ToString()) {
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void DownloadVideo(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        Vod video = Videos.Where(v => v.Id == id).FirstOrDefault();

                        if (video != null)
                        {
                            VodAuthInfo vodAuthInfo = _apiService.GetVodAuthInfo(video.Id);

                            if (!vodAuthInfo.Privileged && vodAuthInfo.SubOnly)
                            {
                                if (_isAuthenticatedSubOnly)
                                {
                                    _dialogService.ShowMessageBox(
                                        "This video is sub-only but you are not subscribed to the channel!", "Download",
                                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                else
                                {
                                    _dialogService.ShowMessageBox(
                                        "This video is sub-only! You need to enable sub-only video download support first!",
                                        "Download", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }

                                return;
                            }

                            Dictionary<VodQuality, string> playlistInfo =
                                _apiService.GetPlaylistInfo(id, vodAuthInfo);
                            List<VodQuality> qualities = playlistInfo.Keys.OrderBy(q => q).ToList();

                            Preferences currentPrefs = _preferencesService.CurrentPreferences.Clone();

                            VodQuality selectedQuality =
                                QualitySelector.Select(qualities, currentPrefs.DownloadDefaultQuality);

                            string folder = currentPrefs.DownloadSubfoldersForFav &&
                                            _preferencesService.IsChannelInFavourites(video.Channel)
                                ? Path.Combine(currentPrefs.DownloadFolder, video.Channel)
                                : currentPrefs.DownloadFolder;

                            string filename = _filenameService.SubstituteWildcards(currentPrefs.DownloadFileName, video,
                                selectedQuality);
                            filename = _filenameService.EnsureExtension(filename,
                                currentPrefs.DownloadDisableConversion);

                            DownloadParameters downloadParams = new DownloadParameters(video, qualities,
                                selectedQuality, folder, filename, currentPrefs.DownloadDisableConversion);

                            if (video.StartTime.HasValue)
                            {
                                downloadParams.CropStartTime = video.StartTime.Value;
                            }

                            _navigationService.ShowDownload(downloadParams);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        public void ShowSearch()
        {
            try
            {
                lock (_commandLockObject)
                {
                    _navigationService.ShowSearch();
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        protected override List<MenuCommand> BuildMenu()
        {
            List<MenuCommand> menuCommands = base.BuildMenu();

            if (menuCommands == null)
            {
                menuCommands = new List<MenuCommand>();
            }

            menuCommands.Add(new MenuCommand(SeachCommnad, "New Search", "fa-solid fa-magnifying-glass"));

            return menuCommands;
        }

        #endregion Methods

        #region EventHandlers

        private void SearchService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;

            FirePropertyChanged(propertyName);

            if (propertyName.Equals(nameof(Videos)))
            {
                ScrollPosition = Vector.Zero;
            }
        }

        #endregion EventHandlers
    }
}