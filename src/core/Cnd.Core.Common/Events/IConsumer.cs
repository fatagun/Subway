using System.Threading.Tasks;

namespace Cnd.Core.Common
{
	public interface IConsumer<in T>
	{
		Task HandleAsync(T eventMessage);
		int Order { get; }
	}
}