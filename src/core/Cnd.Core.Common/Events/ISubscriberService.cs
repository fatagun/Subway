using System.Collections.Generic;

namespace Cnd.Core.Common
{
    public interface ISubscriberService
    {
        IList<IConsumer<T>> GetSubscribers<T>();
    }
}
