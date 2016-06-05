using System;
using NUnit.Framework;
using WaffleOffer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WaffleOffer.Controllers;
using System.Data.Common;
using System.Collections.Generic;

namespace BookFliprUnitTests
{
    [TestFixture]
    public class UnitTest1
    {
        DbConnection connection;
        TradeRepository repo;
        [SetUp]
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
            repo.ClearRepository();
        }

        [Test]
        public void GetHavesTest()
        {
            //get haves from a user
            
            var haves = repo.GetHavesForUsername("Trader1");
            Assert.GreaterOrEqual(haves.Count, 1);
            Assert.AreEqual(haves[0].Title, "Doodad");
        }

        [Test]
        public void GetUserTest()
        {
            var trader = repo.GetUserByUserName("TraderA");
            Assert.NotNull(trader);
        }

        [Test]
        public void CreateTradeTest()
        {
            var trade = new Trade()
            {
                SendingTraderId = repo.GetUserByUserName("TraderA").Id,
                ReceivingTraderId = repo.GetUserByUserName("Trader1").Id,
                Submitted = true,
                LastModified = DateTime.Now,
                
            };
            trade.Items = new List<Item>() { };
            trade.Items.AddRange(repo.GetHavesForUsername("TraderA"));
            trade.Items.AddRange(repo.GetHavesForUsername("Trader1"));
            repo.CreateTrade(trade);

            var savedTrade = repo.GetTradeById(1);
            Assert.NotNull(savedTrade);
        }

        [Test]
        public void UpdateTradeTest()
        {
            //create trade
            var trade = new Trade()
            {
                SendingTraderId = repo.GetUserByUserName("TraderA").Id,
                ReceivingTraderId = repo.GetUserByUserName("Trader1").Id,
                Submitted = true,
                LastModified = DateTime.Now,

            };
            var items = new List<Item>() { };
            items.AddRange(repo.GetHavesForUsername("TraderA"));
            items.AddRange(repo.GetHavesForUsername("Trader1"));
            int itemCount = items.Count;
            trade.Items = items;
            //save it
            repo.CreateTrade(trade);

            //get trade from database
            var savedTrade = repo.GetTradeById(1);
            Assert.NotNull(savedTrade);

            //remove an item from list
            items.RemoveAt(items.Count - 1);
            savedTrade.Items = items;
            //clone and update trade
            var updatedTrade = new Trade(savedTrade);
            repo.UpdateTradedItems(updatedTrade);

            //get trade back again
            savedTrade = repo.GetTradeById(updatedTrade.TradeId);
            Assert.AreEqual(itemCount - 1, updatedTrade.Items.Count);
        }
    }
}
