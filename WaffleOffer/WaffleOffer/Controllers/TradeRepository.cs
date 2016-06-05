using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class TradeRepository
    {
        private readonly UserManager<AppUser> userManager;

        private WaffleOfferContext db;
        //constructor
        public TradeRepository(WaffleOfferContext dbContext, UserManager<AppUser> userManager)
        {
            db = dbContext;
            this.userManager = userManager;
        }

        //database methods
        //get a trade
        public Trade GetTradeById(int? tradeId)
        {
            return (from t in db.Trades.Include("Items")
                        .Include("SendingTrader").Include("ReceivingTrader")
                    where t.TradeId == tradeId
                    select t).FirstOrDefault();
        }
        //get a trader's Haves
        public List<Item> GetHavesForUsername(string username)
        {
            return (from i in db.Items
             where i.ListingUser == username && i.ListingType == Item.ItemType.Have
             select i).ToList();
        }
        //get AppUser by username
        public AppUser GetUserByUserName(string username)
        {
            return userManager.FindByName(username);
        }
        //get Item by id
        public Item GetItemById(int? id)
        {
            return db.Items.Find(id);
        }

        public void CreateTrade(Trade trade)
        {
            //add to database
            db.Trades.Add(trade);
            db.SaveChanges();
        }

        public void RemoveTradeById(int? id)
        {
            db.Trades.Remove(GetTradeById(id));
            db.SaveChanges();
        }

        public void UpdateTradedItems(Trade trade)
        {
            //remove trade from database by id
            RemoveTradeById(trade.TradeId);
            //add model back in
            CreateTrade(trade);
        }

        public List<Trade> GetPendingTradesForUsername(string username)
        {
            return (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                    where (username == t.ReceivingTrader.UserName
                    || username == t.SendingTrader.UserName)
                    orderby t.LastModified descending
                    select t).ToList();
        }

        public List<Trade> GetTradeHistoryForUsername(string username)
        {
            return (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                    where (username == t.ReceivingTrader.UserName
                    || username == t.SendingTrader.UserName)
                    && (t.Canceled || t.Rejected || (t.SenderRating > 0 && t.ReceiverRating > 0))
                    orderby t.LastModified descending
                    select t).ToList();
        }

        public List<Trade> GetTradeRatingsForUsername(string username)
        {
            return (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                    where (username == t.ReceivingTrader.UserName && t.SenderRating > 0)
                    || (username == t.SendingTrader.UserName && t.ReceiverRating > 0)
                    orderby t.LastModified descending
                    select t).ToList();
        }

        public List<Trade> GetAllTrades()
        {
            return (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                    orderby t.LastModified descending
                    select t).ToList();
        }

        //drop database tables
        public void ClearRepository()
        {
            //re-initialize database
            db.Roles.Add(new IdentityRole("Admin"));
            db.Roles.Add(new IdentityRole("Trader"));
            db.SaveChanges();

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
                Title = "Doodad",
                Author = "A girl",
                Description = "Looks like a pipe or a pistol, depending on how you look at it.",
                Quality = 5,
                ListingType = Item.ItemType.Have,
                ListingUser = "Trader1"
            };
            var item2 = new Item()
            {
                Title = "Thingamajig",
                Author = "A girl",
                Description = "Looks like a cross between a buzzsaw and an eggbeater.",
                Quality = 4,
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderA"
            };
            var item3 = new Item()
            {
                Title = "Stuff",
                Author = "A girl",
                Description = "Big pile of odds and ends.",
                Quality = 2,
                ListingType = Item.ItemType.Have,
                ListingUser = "Trader1"
            };
            var item4 = new Item()
            {
                Title = "Widget",
                Author = "A girl",
                Description = "A clockwork pizza slicer.",
                Quality = 3,
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderA"
            };
            db.Items.Add(item1);
            db.Items.Add(item2);
            db.Items.Add(item3);
            db.Items.Add(item4);
            db.SaveChanges();
        }

        private AppUser InitUser(AppUser user, string password, string roleName)
        {
            PasswordHasher hasher = new PasswordHasher();
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