using System.Collections.Generic;
using System.Collections.ObjectModel;
using PurpleLeech.Core.Models;

namespace PurpleLeech.Services.Interfaces
{
    public interface IApiService
    {
        bool ValidateAuthentication(string accessToken, bool subOnly);

        void RevokeAuthentication(string accessToken, bool subOnly);

        VodAuthInfo GetVodAuthInfo(string id);

        Dictionary<VodQuality, string> GetPlaylistInfo(string vodId, VodAuthInfo vodAuthInfo);

        bool ChannelExists(string channel);

        string GetChannelIdByName(string channel);

        ObservableCollection<Vod> Search(SearchParameters searchParams);

        /// <summary>
        /// Returns the live streams of the given channels, keyed by login. Offline channels are not included.
        /// </summary>
        Dictionary<string, LiveStreamInfo> GetLiveStreams(IEnumerable<string> logins);

        /// <summary>
        /// Returns the archive VOD that belongs to the given stream, or null if it does not exist (yet)
        /// </summary>
        Vod GetArchiveVideoForStream(string userId, string streamId);
    }
}