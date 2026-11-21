using System.Threading.Tasks;

namespace PurplePlunder.Services.Interfaces
{
    public interface IAuthListener
    {
        Task StartListenForToken();
    }
}