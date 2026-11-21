using System;
using System.IO;
using PurplePlunder.Services.Interfaces;
using PurplePlunder.Shared.Reflection;

namespace PurplePlunder.Services.Services
{
    internal class FolderService : IFolderService
    {
        #region Fields

        /// <summary>
        /// Earlier product names whose settings are taken over (released as Purple Leech 4.0.0 and 4.0.1)
        /// </summary>
        private static readonly string[] LegacyProductNames = { "Purple Leech" };

        private string appDataFolder;
        private string downloadsTempFolder;
        private string downloadsFolder;

        #endregion Fields

        #region Methods

        public string GetAppDataFolder()
        {
            if (string.IsNullOrWhiteSpace(appDataFolder))
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string productName = AssemblyUtil.Get.GetProductName();
                appDataFolder = Path.Combine(appData, productName);

                MigrateLegacyAppDataFolder(appData, appDataFolder);
            }

            return appDataFolder;
        }

        /// <summary>
        /// Keeps login, preferences and monitored channels after a rename by moving the old settings folder
        /// </summary>
        private static void MigrateLegacyAppDataFolder(string appData, string target)
        {
            if (Directory.Exists(target))
            {
                return;
            }

            foreach (string legacyName in LegacyProductNames)
            {
                string legacy = Path.Combine(appData, legacyName);

                if (Directory.Exists(legacy))
                {
                    try
                    {
                        Directory.Move(legacy, target);
                    }
                    catch
                    {
                        // Starting with fresh settings is better than not starting at all
                    }

                    return;
                }
            }
        }

        public string GetTempFolder()
        {
            if (string.IsNullOrWhiteSpace(downloadsTempFolder))
            {
                downloadsTempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
            }

            return downloadsTempFolder;
        }

        public string GetDownloadFolder()
        {
            if (string.IsNullOrWhiteSpace(downloadsFolder))
            {
                downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }

            return downloadsFolder;
        }

        #endregion Methods
    }
}