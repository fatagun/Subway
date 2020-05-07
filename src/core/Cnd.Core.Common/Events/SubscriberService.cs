using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Cnd.Core.Common
{
	public class SubscriberService : ISubscriberService
    {
        private readonly IServiceProvider _serviceProvider;
        public SubscriberService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public virtual IList<IConsumer<T>> GetSubscribers<T>()
        {
            // var iconsumer = typeof(IConsumer<T>);
            // var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
			                    //  .Where(p => iconsumer.IsAssignableFrom(p) && p.IsClass);

            var types = _serviceProvider.GetServices<IConsumer<T>>();

            var consumers = new List<IConsumer<T>>();

			foreach (var type in types)
            {
				consumers.Add(type);
            }

            return consumers;
        }
    }
}
