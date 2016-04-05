using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public ProfileController()
            : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public ProfileController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        //
        // GET: /Profile/
        [Authorize]
        public ActionResult Index(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                return RedirectToAction("Index", new { userName = User.Identity.GetUserName() });
            }

            var model = userManager.FindByName(userName);
            if (model != null)
                return View(model);
            else
                return HttpNotFound("Profile not found");
        }
    }
}