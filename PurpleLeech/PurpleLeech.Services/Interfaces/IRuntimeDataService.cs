using PurpleLeech.Core.Models;

namespace PurpleLeech.Services.Interfaces
{
    public interface IRuntimeDataService
    {
        RuntimeData RuntimeData { get; }

        void Save();
    }
}