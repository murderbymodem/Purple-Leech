using System.Threading.Tasks;

namespace PurpleLeech.Services.Interfaces
{
    public interface IAuthListener
    {
        Task StartListenForToken();
    }
}