using PurplePlunder.Core.Models;

namespace PurplePlunder.Services.Interfaces
{
    public interface IUpdateService
    {
        UpdateInfo CheckForUpdate();
    }
}