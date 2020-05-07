using System.Threading.Tasks;

namespace Cnd.Core.Common
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T message);
    }
}
