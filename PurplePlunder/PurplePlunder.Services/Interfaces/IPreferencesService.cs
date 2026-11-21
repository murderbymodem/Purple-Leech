using System.Collections.Generic;
using PurplePlunder.Core.Models;

namespace PurplePlunder.Services.Interfaces
{
    public interface IPreferencesService
    {
        Preferences CurrentPreferences { get; }
        IEnumerable<string> AvailableThemes { get; }

        bool IsChannelInFavourites(string channel);

        void Save(Preferences preferences);

        Preferences CreateDefault();
    }
}