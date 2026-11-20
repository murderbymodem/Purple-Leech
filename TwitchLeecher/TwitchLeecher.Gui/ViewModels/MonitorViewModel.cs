using System.Collections.ObjectModel;
using System.Windows.Input;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Gui.Interfaces;
using TwitchLeecher.Gui.Types;
using TwitchLeecher.Services.Interfaces;
using TwitchLeecher.Shared.Commands;

namespace TwitchLeecher.Gui.ViewModels
{
    public class MonitorViewModel : ViewModelBase
    {
        #region Fields

        private readonly IChannelMonitorService _monitorService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        private string _newChannel;
        private bool _isAdding;

        private ICommand _addChannelCommand;
        private ICommand _removeChannelCommand;
        private ICommand _checkNowCommand;
        private ICommand _showDownloadsCommand;

        #endregion Fields

        #region Constructors

        public MonitorViewModel(
            IChannelMonitorService monitorService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _monitorService = monitorService;
            _dialogService = dialogService;
            _navigationService = navigationService;

            _monitorService.Channels.CollectionChanged += (_, _) => FirePropertyChanged(nameof(HasNoChannels));
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<MonitoredChannel> Channels => _monitorService.Channels;

        public bool HasNoChannels => _monitorService.Channels.Count == 0;

        public bool IsEnabled
        {
            get { return _monitorService.IsEnabled; }
            set
            {
                _monitorService.IsEnabled = value;
                FirePropertyChanged(nameof(IsEnabled));
            }
        }

        public string Description =>
            $"Twitch Leech checks these channels every {_monitorService.PollIntervalSeconds} seconds. " +
            "When one goes live, its VOD is recorded while the stream is running, so the download is " +
            "finished moments after the stream ends, before Twitch mutes copyrighted audio. " +
            "Twitch Leech has to stay open for this to work, and your PC is kept awake while recording.";

        public string NewChannel
        {
            get { return _newChannel; }
            set { SetProperty(ref _newChannel, value); }
        }

        public bool IsAdding
        {
            get { return _isAdding; }
            private set { SetProperty(ref _isAdding, value); }
        }

        public ICommand AddChannelCommand => _addChannelCommand ??= new DelegateCommand(AddChannel);

        public ICommand RemoveChannelCommand => _removeChannelCommand ??= new DelegateCommand<string>(RemoveChannel);

        public ICommand CheckNowCommand => _checkNowCommand ??= new DelegateCommand(() => _monitorService.CheckNow());

        public ICommand ShowDownloadsCommand =>
            _showDownloadsCommand ??= new DelegateCommand(() => _navigationService.ShowDownloads());

        #endregion Properties

        #region Methods

        private async void AddChannel()
        {
            string channel = NewChannel;

            if (string.IsNullOrWhiteSpace(channel) || IsAdding)
            {
                return;
            }

            IsAdding = true;

            try
            {
                // Checking that the channel exists is a network call, keep the UI responsive
                await Task.Run(() => _monitorService.AddChannel(channel));

                NewChannel = null;
            }
            catch (ArgumentException ex)
            {
                await _dialogService.ShowMessageBox(ex.Message, "Monitor", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        private void RemoveChannel(string login)
        {
            try
            {
                _monitorService.RemoveChannel(login);
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        #endregion Methods
    }
}
