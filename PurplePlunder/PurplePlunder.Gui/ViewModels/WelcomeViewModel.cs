using PurplePlunder.Shared.Extensions;
using PurplePlunder.Shared.Reflection;

namespace PurplePlunder.Gui.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
        #region Constructors

        public WelcomeViewModel()
        {
            AssemblyUtil au = AssemblyUtil.Get;

            ProductName = au.GetProductName() + " " + au.GetAssemblyVersion().Trim();
        }

        #endregion Constructors

        #region Properties

        public string ProductName { get; }

        #endregion Properties
    }
}