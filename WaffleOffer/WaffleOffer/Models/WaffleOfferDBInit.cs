using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    
    //public class WaffleOfferDBInit : DropCreateDatabaseAlways<WaffleOfferDBContext>
    //public class WaffleOfferDBInit : CreateDatabaseIfNotExists<WaffleOfferDBContext>
    //public class WaffleOfferDBInit : CreateDatabaseIfNotExists<WaffleOffeDBrContext>
    public class WaffleOfferDBInit : DropCreateDatabaseIfModelChanges<WaffleOfferDBContext>
    {
        UserManager<AppUser> userManager;
        PasswordHasher hasher = new PasswordHasher();

        //protected override void Seed(WaffleOfferDBContext context)
        protected override void Seed(WaffleOfferDBContext context)
        {

            userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
            context.Roles.Add(new IdentityRole("Admin"));
            context.Roles.Add(new IdentityRole("Trader"));
            //create admin account
            var admin = new AppUser()
            {
                UserName = "Admin",
                FirstName = "Bossy",
                LastName = "McBossFace",
                Email = "Admin@WaffleOffer.com",
                ZipCode = "91820",
                TraderAccount = new Trader()
            };
            InitUser(admin, "IAmTheLaw", "Admin");
            //create some regular users
            var user1 = new AppUser()
            {
                UserName = "Trader1",
                FirstName = "Gabriel",
                LastName = "Sanchez",
                Email = "ItHitsTheFan@Gmail.com",
                ZipCode = "97444"
            };
            InitUser(user1, "Password", "Trader");
            //populate database
            var user2 = new AppUser()
            {
                UserName = "TraderA",
                FirstName = "Lily",
                LastName = "Hardcastle",
                Email = "Stonewall@Yahoo.com",
                ZipCode = "97507"
            };
            InitUser(user2, "Password", "Trader");
            var user3 = new AppUser()
            {
                UserName = "TraderAlpha",
                FirstName = "Heinrich",
                LastName = "Kalteisen",
                Email = "ColdSteel@Hotmail.com",
                ZipCode = "97243"
            };
            InitUser(user3, "Password", "Trader");
            var user4 = new AppUser()
            {
                UserName = "TraderPrime",
                FirstName = "Melissa",
                LastName = "Caito",
                Email = "MelissaCaito@MCaito.com",
                ZipCode = "97997"
            };
            InitUser(user4, "Password", "Trader");

            //seed some items
            var item1 = new Item()
            {
                Name = "Doodad",
                Description = "Looks like a pipe or a pistol, depending on how you look at it.",
                Quantity = 1,
                Quality = 5,
                Units = "Thing",
                ListingType = Item.ItemType.Have,
                ListingUser = "Trader1"
            };
            var item2 = new Item()
            {
                Name = "Thingamajig",
                Description = "Looks like a cross between a buzzsaw and an eggbeater.",
                Quantity = 1,
                Quality = 4,
                Units = "Thing",
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderA"
            };
            var item3 = new Item()
            {
                Name = "Stuff",
                Description = "Big pile of odds and ends.",
                Quantity = 23,
                Quality = 2,
                Units = "Pounds",
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderAlpha"
            };
            var item4 = new Item()
            {
                Name = "Widget",
                Description = "A clockwork pizza slicer.",
                Quantity = 3,
                Quality = 3,
                Units = "Thing",
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderPrime"
            };
            context.Items.Add(item1);
            context.Items.Add(item2);
            context.Items.Add(item3);
            context.Items.Add(item4);
            base.Seed(context);
        }

        private AppUser InitUser(AppUser user, string password, string roleName)
        {

            user.PasswordHash = hasher.HashPassword(password);

            var oldUser = userManager.FindByName(user.UserName);

            if (oldUser == null)
            {
                userManager.Create(user, password);
                userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            }
            else
            {
                oldUser.PasswordHash = user.PasswordHash;
                oldUser.Email = user.Email;

                user = oldUser;
            }

            //set role
            userManager.AddToRole(user.Id, roleName);

            return user;
        }

    }
}