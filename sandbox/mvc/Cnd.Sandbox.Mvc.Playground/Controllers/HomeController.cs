using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cnd.Sandbox.Mvc.Playground.Models;
using Cnd.Cache.Abstractions;
using System.Threading;
using Cnd.Cache.Redis;
using Cnd.Cache.InMemory;
using Cnd.Core.Common;
using System;

namespace Cnd.Sandbox.Mvc.Playground.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCacheProvider _cache;
        private readonly OldInMemoryCacheProvider _oldCache;
        private readonly IRedisCacheProvider _redis;
        private readonly IGuidGenerator _guidgen;
        private readonly IEventPublisher _eventPublisher;
        private static int counter = 0;

        public HomeController(ILogger<HomeController> logger,
                              IMemoryCacheProvider cache,
                              IRedisCacheProvider redis,
                              IGuidGenerator guidgen,
                              OldInMemoryCacheProvider oldcache,
                              IEventPublisher eventpublisher)
        {
            _logger = logger;
            _cache = cache;
            _redis = redis;
            _oldCache = oldcache;
            _guidgen = guidgen;
            _eventPublisher = eventpublisher;
        }

        public IActionResult Index()
        {
            var g = _guidgen.Generate();
            //var s = _redis.GetSubscriber();
            //s.Publish("foo", "test : "+g);

            return View();
        }

        public IActionResult UserProfile()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PersonViewModel personViewModel)
        {
            PersonContainer.user.Name = personViewModel.Name;
            PersonContainer.user.Age = personViewModel.Age;
            PersonContainer.user.Gender = personViewModel.Gender;

            await _redis.GetSubscriber().PublishAsync("UserProfileUpdated1", true);

            return RedirectToAction("UserProfile");
        }

        public IActionResult EditWithEvents()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditWithEvents(PersonViewModel personViewModel)
        {
            PersonContainer.user.Name = personViewModel.Name;
            PersonContainer.user.Age = personViewModel.Age;
            PersonContainer.user.Gender = personViewModel.Gender;

            await _eventPublisher.PublishAsync(new Event<Person>(PersonContainer.user));
            return RedirectToAction("UserProfile");
        }

        public async Task<IActionResult> Populate()
        {
            await _redis.SetStringAsync("firat", PersonContainer.user);
            await _redis.GetSubscriber().SubscribeAsync("UserProfileUpdated1", async (channel, message)=>
            {
                if(message.HasValue)
                {
                    Console.WriteLine("in subscription.");
                    await _redis.SetStringAsync("firat", PersonContainer.user);
                }
            });

            return Ok("Populated redis with user data");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Setredis()
        {
            var count = Interlocked.Increment(ref counter);

            await _redis.SetStringAsync($"foo-{count}", new Test { Name = $"bar{count}" });

            return Content($"{count}");
        }

        public IActionResult Set()
        {
            var count = Interlocked.Increment(ref counter);

            _cache.Set($"foo-{count}", new Test { Name = $"bar{count}" });

            return Content($"{count}");
        }

        public IActionResult Setreverse()
        {
            var count = Interlocked.Decrement(ref counter);

            _cache.Set($"foo-{count}", new Test { Name = $"bar{count}" });

            return Content($"{count}");
        }

        public IActionResult Seto()
        {
            var count = Interlocked.Increment(ref counter);

            _oldCache.Set($"foo-{count}", new Test { Name = $"bar{count}" });

            return Content($"{count}");
        }

        public IActionResult Setor()
        {
            var count = Interlocked.Decrement(ref counter);

            _oldCache.Set($"foo-{count}", new Test { Name = $"bar{count}" });

            return Content($"{count}");
        }

        public IActionResult Get(string i)
        {
            var res = _cache.Get<Test>($"foo-{i}");

            return Ok(res);
        }

        public IActionResult Geto(string i)
        {
            var res = _oldCache.Get<Test>($"foo-{i}");

            return Ok(res);
        }

        public IActionResult GetCounter()
        {
            return Ok(counter);
        }

        public IActionResult Count() => Ok(_cache.Count());

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class Test
    {
        public string Name { get; set; }
        public int Age {get;set;} = 10;
        public string Foo {get;set;} ="foo";
        public string Bar {get;set;} ="bar";
        public string Help {get;set;} ="test";
    }

}