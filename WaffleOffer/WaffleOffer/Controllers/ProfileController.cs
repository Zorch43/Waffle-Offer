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

	}
}