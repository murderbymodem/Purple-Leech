using System;
using PurplePlunder.Shared.Notification;

namespace PurplePlunder.Core.Models
{
    public class MonitoredChannel : BindableBase
    {
        #region Fields

        private string _status = "Waiting for first check";
        private bool _isLive;
        private bool _isCapturing;
        private DateTime? _lastChecked;

        #endregion Fields

        #region Constructors

        public MonitoredChannel(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException(nameof(login));
            }

            Login = login.Trim().ToLowerInvariant();
        }

        #endregion Constructors

        #region Properties

        public string Login { get; }

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public bool IsLive
        {
            get { return _isLive; }
            set { SetProperty(ref _isLive, value); }
        }

        public bool IsCapturing
        {
            get { return _isCapturing; }
            set { SetProperty(ref _isCapturing, value); }
        }

        public DateTime? LastChecked
        {
            get { return _lastChecked; }
            set { SetProperty(ref _lastChecked, value); }
        }

        /// <summary>
        /// The stream that a live capture was already started for, so it is not started twice.
        /// </summary>
        public string CapturedStreamId { get; set; }

        #endregion Properties
    }
}
