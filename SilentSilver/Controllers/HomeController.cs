using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentSilver.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index ()
        {
            var title = ConfigurationManager.AppSettings[ "PARAM1" ];
            if( String.IsNullOrWhiteSpace( title) )
                ViewBag.Title = "Home Page";
            else
                ViewBag.Title = title;

            ViewBag.Counter = WebApiApplication.ValueRequestCount;
            return View ();
        }
    }
}
