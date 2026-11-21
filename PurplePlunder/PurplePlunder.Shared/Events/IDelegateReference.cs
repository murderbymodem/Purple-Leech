using System;

namespace PurplePlunder.Shared.Events
{
    public interface IDelegateReference
    {
        #region Properties

        Delegate Target { get; }

        #endregion Properties
    }
}