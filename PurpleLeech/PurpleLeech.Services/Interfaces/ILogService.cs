using System;

namespace PurpleLeech.Services.Interfaces
{
    public interface ILogService
    {
        string LogException(Exception ex);
    }
}