using System;

namespace PurplePlunder.Services.Interfaces
{
    public interface ILogService
    {
        string LogException(Exception ex);
    }
}