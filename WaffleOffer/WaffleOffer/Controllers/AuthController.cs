using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class AuthController : Controller
    {

        private readonly UserManager<AppUser> userManager;

        public AuthController() : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public AuthController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        //
        // GET: /Auth/
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogIn(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = userManager.Find(model.UserName, model.Password);

            if (user != null)
            {
                var identity = userManager.CreateIdentity(
                    user, DefaultAuthenticationTypes.ApplicationCookie);

                GetAuthenticationManager().SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = model.Nickname,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ZipCode = model.ZipCode
            };

            var result = userManager.Create(user, model.Password);

            if (result.Succeeded)
            {
                //userManager.AddToRole(user.Id, "Alive");
                SignIn(user);
                return RedirectToAction("index", "home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }

        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }

        private void SignIn(AppUser user)
        {
            var identity = userManager.CreateIdentity(
                user, DefaultAuthenticationTypes.ApplicationCookie);

            GetAuthenticationManager().SignIn(identity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}