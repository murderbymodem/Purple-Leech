using System;
using System.Collections.Generic;
using PurplePlunder.Core.Models;

namespace PurplePlunder.Services.Interfaces
{
    public interface IProcessingService
    {
        string FFMPEGExe { get; }

        IEnumerable<string> ConcatParts(Action<string> log, Action<string> setStatus, Action<double> setProgress, VodPlaylist vodPlaylist, string concatFile);

        void ConvertVideo(Action<string> log, Action<string> setStatus, Action<double> setProgress, Action<bool> setIsIndeterminate, string sourceFile, string outputFile, CropInfo cropInfo);
    }
}