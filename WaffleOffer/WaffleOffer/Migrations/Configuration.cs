namespace WaffleOffer.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WaffleOffer.Models.WaffleOfferContext>
    {
        UserManager<AppUser> userManager;
        PasswordHasher hasher = new PasswordHasher();

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WaffleOffer.Models.WaffleOfferContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //context.Database.Delete();
            context.Database.CreateIfNotExists();
            userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
            context.Roles.AddOrUpdate(r => r.Name, new IdentityRole("Admin"), new IdentityRole("Trader"));
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
            /*var user1 = new AppUser()
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
            //load users
            //context.Users.Load();
            //seed some items
            var item1 = new Item()
            {
                Title = "Doodad",
                Description = "Looks like a pipe or a pistol, depending on how you look at it.",
                Quality = 5,
                Author = "The dude",
                ListingType = Item.ItemType.Have,
                ListingUser = "Trader1"
            };
            var item2 = new Item()
            {
                Title = "Thingamajig",
                Description = "Looks like a cross between a buzzsaw and an eggbeater.",
                Quality = 4,
                Author = "Stephen King",
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderA"
            };
            var item3 = new Item()
            {
                Title = "Stuff",
                Description = "Big pile of odds and ends.",
                Quality = 2,
                Author = "Beverly Cleary",
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderAlpha"
            };
            var item4 = new Item()
            {
                Title = "Widget",
                Description = "A clockwork pizza slicer.",
                Quality = 3,
                Author = "Chuck Palahniuk",
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderPrime"
            };
            
            context.Items.AddOrUpdate(i => i.Title, item1, item2, item3, item4);

            var thread1 = new Thread() { ThreadID = 0 };
            var thread2 = new Thread() { ThreadID = 1 };

            context.Threads.AddOrUpdate(t => t.ThreadID, thread1, thread2);

            DateTime date1 = new DateTime(2016, 6, 1, 1, 1, 1);
            DateTime date2 = new DateTime(2016, 6, 1, 2, 2, 2);
            DateTime date3 = new DateTime(2016, 6, 1, 3, 3, 3);

            var msg1 = new Message()
            {
                SenderID = user2.Id,
                RecipientID = user3.Id,
                Subject = "RE: The Autobiography of Fred Durst",
                Body = "Is Fred Durst related to Robert Durst? Because the musical travesty that is Limp Bizkit murdered my brain cells and got away with it.",
                DateCreated = date1,
                DateSent = date1,
                Sent = true,
                Copy = false,
                IsReply = false,
                ThreadID = thread1.ThreadID
            };

            var msg2 = new Message()
            {
                SenderID = user2.Id,
                RecipientID = user3.Id,
                Subject = "RE: The Autobiography of Fred Durst",
                Body = "Is Fred Durst related to Robert Durst? Because the musical travesty that is Limp Bizkit murdered my brain cells and got away with it.",
                DateCreated = date1,
                DateSent = date1,
                Sent = true,
                Copy = true,
                IsReply = false,
                ThreadID = thread1.ThreadID
            };

            var msg3 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user2.Id,
                Subject = "RE: The Dursts",
                Body = "No, but I think you can understand why I'm trying to unload this stupid book...",
                DateCreated = date2,
                DateSent = date2,
                Sent = true,
                Copy = false,
                IsReply = true,
                ThreadID = thread1.ThreadID
            };

            var msg4 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user2.Id,
                Subject = "RE: The Dursts",
                Body = "No, but I think you can understand why I'm trying to unload this stupid book...",
                DateCreated = date2,
                DateSent = date2,
                Sent = true,
                Copy = true,
                IsReply = true,
                ThreadID = thread1.ThreadID
            };

            var msg5 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user4.Id,
                Subject = "Hi",
                Body = "I like books.",
                DateCreated = date3,
                DateSent = date3,
                Sent = true,
                Copy = false,
                IsReply = false,
                ThreadID = thread2.ThreadID
            };

            var msg6 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user4.Id,
                Subject = "Hi",
                Body = "I like books.",
                DateCreated = date3,
                DateSent = date3,
                Sent = true,
                Copy = true,
                IsReply = false,
                ThreadID = thread2.ThreadID
            };

            context.Messages.AddOrUpdate(m => m.MessageID, msg1, msg2, msg3, msg4, msg5, msg6);*/
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
