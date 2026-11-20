using System;

namespace PurpleLeech.Shared.Events
{
    public interface IDelegateReference
    {
        #region Properties

        Delegate Target { get; }

        #endregion Properties
    }
}