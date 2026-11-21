using System;
using PurplePlunder.Core.Models;

namespace PurplePlunder.Services.Interfaces
{
    public interface IFilenameService
    {
        string SubstituteWildcards(string filename, Vod video, VodQuality quality, TimeSpan? cropStart = null, TimeSpan? cropEnd = null);

        string SubstituteInvalidChars(string filename, string replaceStr);

        string EnsureExtension(string filename, bool disableConversion);
    }
}