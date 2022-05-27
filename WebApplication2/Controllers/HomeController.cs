using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class CheckTest
    {
        public string Code { get; set; }
        public string Guid { get; set; }
    }
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Test()
        {
            var result = Transferer<List<VolEvent>>.TransferGet("https://sbercringe.space/api/v2/actions/getvolevents");
            var i = 0;
            var obj = Transferer<VolEvent>.TransfererPost(new VolEvent() { Name = "test", City = "test", Content = "test" }, "https://sbercringe.space/api/v2/actions/addvolevent");
            var ii = 0;

            return View("Index");
        }

        public ActionResult TestAuth()
        {
            var user = new MyCredentials()
            {
                Username = "test",
                Mac = "testovich",
                Email = "stakan993@gmail.com",
                Guid = Guid.NewGuid().ToString(),
                Reg = true,
            };

            var rd = new MyRouteData<MyCredentials>()
            {
                RouteObject = user,
            };

            var res = Transferer<MyRouteData<MyCredentials>>.TransfererPost(rd, "https://localhost:44374/api/v2/actions/auth");

            var m = new CheckTest()
            {
                Guid = res,
            };
            return View("Index", m);
        }

        public ActionResult Smbt(CheckTest t)
        {
            var user = new MyCredentials()
            {
                ConfirmCode = t.Code,
            };

            var rd = new MyRouteData<MyCredentials>()
            {
                Guid = t.Guid,
                RouteObject = user,
            };
            var res = Transferer<MyRouteData<MyCredentials>>.TransfererPost(rd, "https://localhost:44374/api/v2/actions/authconfirm");

            var m = new CheckTest()
            {
                Guid = res,
            };

            return View("Index", m);
        }
    }
}
