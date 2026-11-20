using System;
using System.Collections.ObjectModel;
using TwitchLeecher.Core.Models;

namespace TwitchLeecher.Services.Interfaces
{
    public interface IChannelMonitorService
    {
        ObservableCollection<MonitoredChannel> Channels { get; }

        bool IsEnabled { get; set; }

        int PollIntervalSeconds { get; }

        /// <summary>
        /// Raised on the UI thread when a live capture was added to the downloads
        /// </summary>
        event Action<string> CaptureStarted;

        void AddChannel(string login);

        void RemoveChannel(string login);

        void CheckNow();
    }
}
