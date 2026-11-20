using PurpleLeech.Core.Models;

namespace PurpleLeech.Services.Interfaces
{
    public interface IUpdateService
    {
        UpdateInfo CheckForUpdate();
    }
}