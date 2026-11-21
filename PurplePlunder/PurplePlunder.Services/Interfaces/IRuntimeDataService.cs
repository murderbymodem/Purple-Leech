using PurplePlunder.Core.Models;

namespace PurplePlunder.Services.Interfaces
{
    public interface IRuntimeDataService
    {
        RuntimeData RuntimeData { get; }

        void Save();
    }
}