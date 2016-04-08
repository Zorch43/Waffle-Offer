using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "A web app for trading your stuff for new stuff.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Here is how to contact us:";

            return View();
        }
    }
}