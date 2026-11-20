using PurpleLeech.Core.Models;

namespace PurpleLeech.Gui.Interfaces
{
    public interface INavigationService
    {
        void ShowAuth();

        void ShowWelcome();

        void ShowLoading();

        void ShowSearch();

        void ShowSearchResults();

        void ShowDownload(DownloadParameters downloadParams);

        void ShowDownloads();

        void ShowMonitor();

        void ShowPreferences();

        void ShowInfo();

        void ShowLog(VodDownload download);

        void NavigateBack();
        void ShowAuthSubOnly();
    }
}