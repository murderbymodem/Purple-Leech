using System;

namespace PurplePlunder.Shared.Events
{
    public interface IEventSubscription
    {
        #region Properties

        SubscriptionToken SubscriptionToken { get; set; }

        #endregion Properties

        #region Methods

        Action<object[]> GetExecutionStrategy();

        #endregion Methods
    }
}