using Ninject.Modules;
using PurplePlunder.Gui.Interfaces;
using PurplePlunder.Gui.Services;

namespace PurplePlunder.Gui.Modules
{
    public class GuiModule : NinjectModule
    {
        #region Methods

        public override void Load()
        {
            Bind<IDialogService>().To<DialogService>().InSingletonScope();
            Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            Bind<INotificationService>().To<NotificationService>().InSingletonScope();
            Bind<ISearchService>().To<SearchService>().InSingletonScope();
        }

        #endregion Methods
    }
}