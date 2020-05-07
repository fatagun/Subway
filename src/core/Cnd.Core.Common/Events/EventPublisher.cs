namespace Cnd.Core.Common
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class EventPublisher : IEventPublisher
    {
		private readonly ISubscriberService _subscriberService;
        private readonly ILogger<EventPublisher> _logger;

		public EventPublisher(ISubscriberService subscriberService, ILogger<EventPublisher> logger)
        {
            _subscriberService = subscriberService;
            _logger = logger;
        }

        public virtual async Task PublishAsync<T>(T message)
        {
			var subscribers = _subscriberService.GetSubscribers<T>();

			foreach(var consumer in subscribers.OrderBy(s=>s.Order))
			{
				await PublishToConsumerAsync(consumer, message);
			}
        }

        protected virtual async Task PublishToConsumerAsync<T>(IConsumer<T> consumer, T eventMessage)
        {
            try
            {
                await consumer.HandleAsync(eventMessage);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, " Exception in handler.");
				throw;
            }
        }
    }
}
