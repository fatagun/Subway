using System.Diagnostics.CodeAnalysis;

namespace Cnd.Core.Common
{
    public class Event<T> where T: IEventEntity
    {
        public Event(T entity)
        {
			Entity = entity;
        }

		public T Entity { get; }
    }
}
