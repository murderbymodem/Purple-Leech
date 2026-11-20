using PurpleLeech.Core.Models;
using PurpleLeech.Gui.Types;

namespace PurpleLeech.Gui.Interfaces
{
    public interface IDialogService
    {
        Task<MessageBoxResult> ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon);

        void ShowAndLogException(Exception ex);

        void ShowFolderBrowserDialog(string folder, Action<bool, string> dialogCompleteCallback);

        void ShowFileBrowserDialog(CommonFileDialogFilter filter, string folder, Action<bool, string> dialogCompleteCallback);

        void ShowUpdateInfoDialog(UpdateInfo updateInfo);

        void SetBusy();
    }
}