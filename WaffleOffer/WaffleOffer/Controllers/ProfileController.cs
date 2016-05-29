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

        private WaffleOfferContext db = new WaffleOfferContext();

        public ProfileController() : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public ProfileController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        //
        // GET: /Profile/userName
        [Authorize]
        public ActionResult Index(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                return RedirectToAction("Index", new { userName = User.Identity.GetUserName() });
            }

            var model = userManager.FindByName(userName);
            model.TraderAccount = new Trader()
            {
                Wants = (from i in db.Items
                         where i.ListingUser == model.UserName && i.ListingType == Item.ItemType.Want
                         select i).ToList(),
                Haves = (from i in db.Items
                         where i.ListingUser == model.UserName && i.ListingType == Item.ItemType.Have
                         select i).ToList()
            };

            if (model != null)
            {
                var profile = new ProfileViewModel(model);

                //calculate rating
                var trades = (from t in db.Trades.Include("SendingTrader").Include("ReceivingTrader")
                                  //where (t.SendingTrader.UserName == model.UserName && t.ReceiverRating > 0)
                                  //   || (t.ReceivingTrader.UserName == model.UserName && t.SenderRating > 0)
                              select t).ToList();
                int ratingSum = 0;
                int count = 0;
                foreach (Trade t in trades)
                {
                    if (t.SendingTrader.UserName == model.UserName && t.ReceiverRating > 0)
                    {
                        ratingSum += t.ReceiverRating;
                        count++;
                    }
                    else if (t.ReceivingTrader.UserName == model.UserName && t.SenderRating > 0)
                    {
                        ratingSum += t.SenderRating;
                        count++;
                    }
                        
                }

                profile.Rating = (double)ratingSum / count;
                return View(profile);
            }

            else
            {
                return HttpNotFound("Profile not found");
            }
                
        }

        public ActionResult Browse()
        {
            List<ProfileViewModel> profiles = new List<ProfileViewModel>();

            // Retrieve all user profiles, in ascending order by zipcode (for now)
            // Currently includes admins
            List<AppUser> allUsers = (from p in db.Users
                                     orderby p.ZipCode ascending
                                     select p).ToList();

            // Loop through the retrieved profiles and create a ProfileViewModel object
            // for each AppUser user object. Add each object to a list of ProfileViewModel
            // objects.
            foreach (AppUser user in allUsers)
            {
                var model = userManager.FindByName(user.UserName);
                if (model != null)
                {
                    ProfileViewModel profile = new ProfileViewModel(model);
                    profiles.Add(profile);
                }      
            }

            return View(profiles);
        }

        //GET: /Profile/Edit/userName
        [Authorize]
        public ActionResult Edit(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName) || (User.Identity.Name != userName && !User.IsInRole("Admin")))
            {
                return RedirectToAction("Edit", new { userName = User.Identity.GetUserName() });
            }

            var model = userManager.FindByName(userName);
            if (model != null)
                return View(new ProfileViewModel(model));
            else
                return HttpNotFound("Profile not found");
        }

        //POST: /Profile/Edit/
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var user = userManager.FindByName(model.Nickname);
            if (user != null)
            {
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ZipCode = model.ZipCode;
                user.ProfileText = model.ProfileText;

                userManager.Update(user);
                return RedirectToAction("Index", new { userName = user.UserName });
            }
            return View(model);
            
        }

        [HttpGet]
        public ActionResult ChangePassword(string user)
        {
            //if username does not equal the current user's nickname, redirect to user's own page
            if (user != User.Identity.Name)
            {
                return RedirectToAction("ChangePassword", new { user = User.Identity.Name });
            }

            //get user
            var appUser = userManager.FindByName(user);
            //create viewmodel
            var passwordVM = new ChangePasswordViewModel()
            {
                PasswordHash = appUser.PasswordHash
            };

            //display model
            return View(passwordVM);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            

            //verify model
            if (!ModelState.IsValid)
            {
                return View();
            }

            //verify old password
            var user = userManager.Find(User.Identity.Name, model.OldPassword);
            if (user == null)
            {
                ModelState.AddModelError("", "Old password is incorrect");
                return View();
            }

            //change to new password
            var hasher = new PasswordHasher();
            user.PasswordHash = hasher.HashPassword(model.NewPassword);
            userManager.Update(user);
            //log out
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

	}
}