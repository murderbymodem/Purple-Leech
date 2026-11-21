using System;
using System.Collections.Generic;
using System.IO;

namespace PurplePlunder.Core.Models
{
    /// <summary>
    /// Twitch stores a chunk as "12.ts", "12-unmuted.ts" or "12-muted.ts" (or .mp4). After a stream ends the
    /// originals of muted chunks are deleted and playlists do not always name the file that still exists.
    /// </summary>
    public static class SegmentNames
    {
        /// <summary>
        /// The urls to try for a chunk, original audio first and the muted copy last
        /// </summary>
        public static IEnumerable<string> GetCandidates(string remoteFile)
        {
            int query = remoteFile.IndexOf('?');
            string path = query >= 0 ? remoteFile.Substring(0, query) : remoteFile;
            string suffix = query >= 0 ? remoteFile.Substring(query) : string.Empty;

            string ext = Path.GetExtension(path);
            string stem = path.Substring(0, path.Length - ext.Length);

            if (stem.EndsWith("-unmuted", StringComparison.OrdinalIgnoreCase))
            {
                stem = stem.Substring(0, stem.Length - "-unmuted".Length);
            }
            else if (stem.EndsWith("-muted", StringComparison.OrdinalIgnoreCase))
            {
                stem = stem.Substring(0, stem.Length - "-muted".Length);
            }

            List<string> candidates = new List<string>();

            void Add(string url)
            {
                if (!candidates.Contains(url))
                {
                    candidates.Add(url);
                }
            }

            bool listedIsMuted = path.EndsWith("-muted" + ext, StringComparison.OrdinalIgnoreCase) &&
                                 !path.EndsWith("-unmuted" + ext, StringComparison.OrdinalIgnoreCase);

            if (!listedIsMuted)
            {
                Add(remoteFile);
            }

            Add(stem + ext + suffix);
            Add(stem + "-unmuted" + ext + suffix);
            Add(stem + "-muted" + ext + suffix);

            return candidates;
        }
    }
}
