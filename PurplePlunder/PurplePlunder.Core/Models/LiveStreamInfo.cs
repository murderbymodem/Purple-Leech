using System;

namespace PurplePlunder.Core.Models
{
    public class LiveStreamInfo
    {
        public LiveStreamInfo(string streamId, string userId, string userLogin, string userName, string title, DateTime startedAt)
        {
            StreamId = streamId;
            UserId = userId;
            UserLogin = userLogin;
            UserName = userName;
            Title = title;
            StartedAt = startedAt;
        }

        public string StreamId { get; }

        public string UserId { get; }

        public string UserLogin { get; }

        public string UserName { get; }

        public string Title { get; }

        public DateTime StartedAt { get; }
    }
}
