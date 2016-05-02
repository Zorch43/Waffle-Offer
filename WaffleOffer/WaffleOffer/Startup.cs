using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaffleOffer.Models;

namespace WaffleOffer
{
    public partial class Startup
    {

        public static Func<UserManager<AppUser>> UserManagerFactory { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            // this is the same as before
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login")
            });

            // configure the user manager
            UserManagerFactory = () =>
            {
                /*var usermanager = new UserManager<AppUser>(
                    new UserStore<AppUser>(new WaffleOfferContext()));*/
                var usermanager = new UserManager<AppUser>(
                    new UserStore<AppUser>(new WaffleOfferContext()));
                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<AppUser>(usermanager)
                {
                    RequireUniqueEmail = true,
                    AllowOnlyAlphanumericUserNames = false
                };

                return usermanager;
            };
        }

    }
}