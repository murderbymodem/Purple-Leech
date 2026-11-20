using System.Collections.Generic;
using System.Linq;

namespace PurpleLeech.Core.Models
{
    public class VodSearchResult
    {
        public VodSearchResult(IEnumerable<Vod> results, IEnumerable<string> logs)
        {
            Results = results.ToArray();
            Logs = logs.ToArray();
        }

        public IReadOnlyCollection<Vod> Results { get; }

        public IReadOnlyCollection<string> Logs { get; }
    }
}
