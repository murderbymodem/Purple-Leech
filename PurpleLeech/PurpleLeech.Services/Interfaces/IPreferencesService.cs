using System.Collections.Generic;
using PurpleLeech.Core.Models;

namespace PurpleLeech.Services.Interfaces
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