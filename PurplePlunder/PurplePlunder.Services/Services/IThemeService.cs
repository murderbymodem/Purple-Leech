using System;

namespace PurplePlunder.Services.Services
{
    public interface IThemeService
    {
        event EventHandler StyleChanged;
        void SetTheme(string name);
        string GetTheme();
    }
}