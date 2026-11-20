using Ninject.Modules;
using PurpleLeech.Gui.Interfaces;
using PurpleLeech.Gui.Services;

namespace PurpleLeech.Gui.Modules
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