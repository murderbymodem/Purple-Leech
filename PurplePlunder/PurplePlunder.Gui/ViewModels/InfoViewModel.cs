using System.Diagnostics;
using System.Windows.Input;
using PurplePlunder.Gui.Interfaces;
using PurplePlunder.Shared.Commands;
using PurplePlunder.Shared.Extensions;
using PurplePlunder.Shared.Reflection;

namespace PurplePlunder.Gui.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        #region Fields

        private ICommand _openlinkCommand;

        private readonly IDialogService _dialogService;

        private readonly object _commandLockObject;

        #endregion Fields

        #region Constructors

        public InfoViewModel(IDialogService dialogService)
        {
            AssemblyUtil au = AssemblyUtil.Get;

            ProductName = au.GetProductName() + " " + au.GetAssemblyVersion().Trim();

            _dialogService = dialogService;

            _commandLockObject = new object();
        }

        #endregion Constructors

        #region Properties

        public string ProductName { get; }

        public ICommand OpenLinkCommand
        {
            get
            {
                if (_openlinkCommand == null)
                {
                    _openlinkCommand = new DelegateCommand<string>(OpenLink);
                }

                return _openlinkCommand;
            }
        }

        #endregion Properties

        #region Methods

        private void OpenLink(string link)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (string.IsNullOrWhiteSpace(link))
                    {
                        throw new ArgumentNullException(nameof(link));
                    }

                    var psi = new ProcessStartInfo(link)
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        #endregion Methods
    }
}