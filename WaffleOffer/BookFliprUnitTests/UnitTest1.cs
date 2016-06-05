using System;
using NUnit.Framework;
using WaffleOffer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WaffleOffer.Controllers;
using System.Data.Common;

namespace BookFliprUnitTests
{
    [TestFixture]
    public class UnitTest1
    {
        DbConnection connection;
        TradeRepository repo;
        [OneTimeSetUp]
        public void Init()
        {
            connection = Effort.DbConnectionFactory.CreateTransient();



            // configure the user manager
            Func<UserManager<AppUser>> UserManagerFactory = () =>
            {
                /*var usermanager = new UserManager<AppUser>(
                    new UserStore<AppUser>(new WaffleOfferContext()));*/
                var usermanager = new UserManager<AppUser>(
                    new UserStore<AppUser>(new WaffleOfferContext(connection)));
                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<AppUser>(usermanager)
                {
                    RequireUniqueEmail = true,
                    AllowOnlyAlphanumericUserNames = false
                };

                return usermanager;
            };

            WaffleOfferContext db = new WaffleOfferContext(connection);

            repo = new TradeRepository(db, UserManagerFactory.Invoke());
            //System.IO.Directory.SetCurrentDirectory(@"C:/Users/Timo/Documents/GitHub/Waffle-Offer/WaffleOffer/BookFliprUnitTests/App_Data");
            //AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
            //AppDomain.CurrentDomain.SetData(@"C:/Users/Timo/Documents/GitHub/Waffle-Offer/WaffleOffer/BookFliprUnitTests/App_Data");
        }

        [SetUp]
        public void ClearDatabase()
        {
            repo.ClearRepository();
        }

        [Test]
        public void GetHavesTest()
        {
            //get haves from a user
            var trader = repo.GetUserByUserName("Trader1");
            Assert.NotNull(trader);
            var haves = repo.GetHavesForUsername("Trader1");
            //Assert.GreaterOrEqual(haves.Count, 1);
            //Assert.AreEqual(haves[0].Title, "Doodad");
        }
    }
}
