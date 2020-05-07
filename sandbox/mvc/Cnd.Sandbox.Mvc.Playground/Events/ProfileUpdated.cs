using System;
using System.Threading.Tasks;
using Cnd.Cache.Redis;
using Cnd.Core.Common;
using Cnd.Sandbox.Mvc.Playground.Models;

namespace Cnd.Sandbox.Mvc.Playground.Events
{

    public class RandomConsumer : IConsumer<Event<Person>>
    {
        public int Order => 2;


        public async Task HandleAsync(Event<Person> eventMessage)
        {
            Console.WriteLine("Hello");
            await Task.CompletedTask;
        }
    }

    public class ProfileUpdatedConsumer : IConsumer<Event<Person>>
    {
        public int Order => 1;

        private readonly IRedisCacheProvider _redis;
        public ProfileUpdatedConsumer(IRedisCacheProvider redis)
        {
            _redis = redis;
        }

        public async Task HandleAsync(Event<Person> eventMessage)
        {
            await _redis.RemoveStringAsync("firat");
            await _redis.SetStringAsync("firat", eventMessage.Entity);
        }
    }


}