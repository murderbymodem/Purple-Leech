using System.Collections.Generic;
using System.Linq;

namespace PurplePlunder.Core.Models
{
    public static class QualitySelector
    {
        public static VodQuality Select(List<VodQuality> qualities, DefaultQuality defaultQuality)
        {
            // "Source" means the best available rendition. With enhanced broadcasting the
            // highest resolution (e.g. 1440p HEVC) can be a separate rendition next to "chunked".
            VodQuality sourceQuality = qualities
                .Where(q => !q.IsAudioOnly)
                .OrderByDescending(q => q.VerticalResolution)
                .ThenByDescending(q => q.IsSource)
                .FirstOrDefault() ?? qualities[0];

            if (defaultQuality == null || defaultQuality.IsSource)
            {
                return sourceQuality;
            }

            if (defaultQuality.IsAudioOnly)
            {
                return qualities.FirstOrDefault(q => q.IsAudioOnly) ?? sourceQuality;
            }

            List<VodQuality> visualQualities = qualities.Where(q => !q.IsAudioOnly).ToList();

            int defaultRes = defaultQuality.VerticalResolution;

            // Highest quality at or below the preferred resolution, otherwise the lowest one above it
            return visualQualities
                       .Where(q => q.VerticalResolution <= defaultRes)
                       .OrderByDescending(q => q.VerticalResolution)
                       .FirstOrDefault()
                   ?? visualQualities
                       .Where(q => q.VerticalResolution >= defaultRes)
                       .OrderBy(q => q.VerticalResolution)
                       .FirstOrDefault()
                   ?? sourceQuality;
        }
    }
}
