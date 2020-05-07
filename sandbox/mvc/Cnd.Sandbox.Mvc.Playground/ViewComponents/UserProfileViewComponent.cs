using System;
using System.Threading.Tasks;
using Cnd.Cache.Redis;
using Cnd.Sandbox.Mvc.Playground.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cnd.Sandbox.Mvc.Playground
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly IRedisCacheProvider _redis;
        public UserProfileViewComponent(IRedisCacheProvider redis)
        {
            _redis = redis;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _redis.GetStringAsync<Person>("firat");

            // if(user == null)
            // {
            //     await _redis.SetStringAsync("firat", PersonContainer.user);
            // }

            return View(user);
        }
    }
}