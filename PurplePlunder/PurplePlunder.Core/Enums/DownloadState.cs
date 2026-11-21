using System.ComponentModel;

namespace PurplePlunder.Core.Enums
{
    public enum DownloadState
    {
        Queued,
        Paused,
        Downloading,
        [Description("Completed with warnings")]
        CompletedWithWarning,
        Canceled,
        Error,
        Done
    }
}