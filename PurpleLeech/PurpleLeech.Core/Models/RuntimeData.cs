using System;

namespace PurpleLeech.Core.Models
{
    public class RuntimeData
    {
        #region Properties

        public Version Version { get; set; }

        public AuthInfo AuthInfo { get; set; }

        public MainWindowInfo MainWindowInfo { get; set; }

        #endregion Properties
    }
}