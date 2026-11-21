using PurplePlunder.Core.Events;
using PurplePlunder.Gui.Interfaces;
using PurplePlunder.Services.Interfaces;
using PurplePlunder.Shared.Events;

namespace PurplePlunder.Gui.ViewModels
{
    public class SubOnlyViewModel : ViewModelBase
    {
        private readonly IAuthListener _authListener;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public SubOnlyViewModel(IAuthListener authListener, IEventAggregator eventAggregator,
            INavigationService navigationService)
        {
            _authListener = authListener;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        public override void OnBeforeShown()
        {
            _eventAggregator.GetEvent<SubOnlyAuthChangedEvent>().Subscribe(Changed);
            _authListener.StartListenForToken();
            base.OnBeforeShown();
        }

        public override void OnBeforeHidden()
        {
            _eventAggregator.GetEvent<SubOnlyAuthChangedEvent>().Unsubscribe(Changed);
            base.OnBeforeHidden();
        }

        private void Changed(bool obj)
        {
            _navigationService.NavigateBack();
        }
    }
}