using System;
using System.Collections.ObjectModel;
using PurplePlunder.Core.Models;

namespace PurplePlunder.Services.Interfaces
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

        /// <summary>
        /// Raised on the UI thread with the number of interrupted recordings that were finished on startup
        /// </summary>
        event Action<int> RecordingsRecovered;

        void AddChannel(string login);

        void RemoveChannel(string login);

        void CheckNow();
    }
}
