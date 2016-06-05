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
        public List<Item> GetHavesForTrader(AppUser trader)
        {
            return (from i in db.Items
             where i.ListingUser == trader.UserName && i.ListingType == Item.ItemType.Have
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
    }
}