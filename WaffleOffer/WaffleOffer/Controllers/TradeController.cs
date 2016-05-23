using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;
using Vereyon.Web;

namespace WaffleOffer.Controllers
{
    public class TradeController : Controller
    {
         private readonly UserManager<AppUser> userManager;

        private WaffleOfferContext db = new WaffleOfferContext();

        public TradeController() : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public TradeController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        // GET: Trade
        public ActionResult Index(string partner, int? tradeId)
        {
            if (tradeId != null)
            {
                var trade = (from t in db.Trades.Include("Items")
                        .Include("SendingTrader").Include("ReceivingTrader")
                             where t.TradeId == tradeId
                             select t).FirstOrDefault();

                if (trade != null)
                {
                    if (User.IsInRole("Admin")
                        || User.Identity.GetUserId() == trade.SendingTraderId
                        || User.Identity.GetUserId() == trade.ReceivingTraderId)
                    {
                        //load model up
                        trade.SendingTrader.TraderAccount = new Trader()
                        {
                            Haves = (from i in db.Items
                                     where i.ListingUser == trade.SendingTrader.UserName && i.ListingType == Item.ItemType.Have
                                     select i).ToList()
                        };
                        

                        trade.ReceivingTrader.TraderAccount = new Trader()
                        {
                            Haves = (from i in db.Items
                                     where i.ListingUser == trade.ReceivingTrader.UserName && i.ListingType == Item.ItemType.Have
                                     select i).ToList()
                        };

                        return View(trade);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }

                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            else
            {
                //find partner
                var tradePartner = userManager.FindByName(partner);
                if (tradePartner != null)
                {
                    var model = new Trade()
                    {
                        SendingTrader = userManager.FindByName(User.Identity.Name),
                        Items = new List<Item>(),
                        ReceivingTrader = tradePartner,
                    };

                    //load model up
                    model.SendingTrader.TraderAccount = new Trader()
                    {
                        Haves = (from i in db.Items
                                 where i.ListingUser == model.SendingTrader.UserName && i.ListingType == Item.ItemType.Have
                                 select i).ToList()
                    };

                    model.ReceivingTrader.TraderAccount = new Trader()
                    {
                        Haves = (from i in db.Items
                                 where i.ListingUser == model.ReceivingTrader.UserName && i.ListingType == Item.ItemType.Have
                                 select i).ToList()
                    };

                    //if either partner has no items, redirect to instructions page
                    if (model.SendingTrader.TraderAccount.Haves.Count < 1
                        || model.ReceivingTrader.TraderAccount.Haves.Count < 1)
                    {
                        FlashMessage.Info("Please create an item that you can trade before entering a trade");
                        return RedirectToAction("Create", "Items", new { type = Item.ItemType.Have.ToString() });
                    }

                    return View(model);
                }
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "TradeId, SenderId,ReceiverId,Items")] TradeCreator trade)
        {
            //create Trade
            var model = new Trade()
            {
                SendingTraderId = trade.SenderId,
                ReceivingTraderId = trade.ReceiverId,
                Submitted = true,
                LastModified = DateTime.Now
            };

            var items = new List<Item>();

            //get list of items
            for (int i = 0; i < trade.Items.Count; i++)
            {
                items.Add(db.Items.Find(trade.Items.ElementAt(i)));
            }

            //add traded items to trade
            model.Items = items;

            //check trade for validity: 
            //Removed or Reserved items cannot be part of a submitted trade
            if (!model.IsAcceptable)
            {
                FlashMessage.Warning("Removed or reserved items may not be submitted in trade or counter-offer.  "
                    + "Remove any reserved or removed items from the trade and try again.");
                return View("Index", trade);
            }
            if (trade.TradeId == null)
            {
                //add to database
                db.Trades.Add(model);
                db.SaveChanges();
            }
            else
            {
                //update trade
                //swap sender and receiver
                model.ReceivingTraderId = trade.SenderId;
                model.SendingTraderId = trade.ReceiverId;
                //set trade id
                model.TradeId = trade.TradeId;
                //update in database
                //also update many-to-many relationship with items
                //remove
                db.Trades.Remove(db.Trades.Find(model.TradeId));
                db.SaveChanges();
                //then add back in, with new list of items
                db.Trades.Add(model);
                db.SaveChanges();
            }
            


            //go back to items
            return RedirectToAction("Pending");
        }

        [HttpPost]
        public ActionResult Update(int tradeId, string status)
        {
            Trade trade = (from t in db.Trades.Include("Items").Include("SendingTrader").Include("ReceivingTrader")
                           where t.TradeId == tradeId
                           select t).FirstOrDefault();

            if (trade != null)
            {
                switch (status)
                {
                    case "Accept":
                        trade.Accepted = true;
                        //mark all items as Reserved
                        //if an item was already reserved or removed, remove it
                        var itemList = new List<Item>();
                        foreach (Item i in trade.Items)
                        {
                            if (!i.Reserved && !i.Removed)
                            {
                                i.Reserved = true;
                                itemList.Add(i);
                            }
                        }
                        trade.Items = itemList;
                        break;
                    case "Cancel":
                        trade.Canceled = true;
                        //if trade was previously accepted,
                        //un-reserve all items in trade
                        if (trade.Accepted)
                        {
                            foreach (Item i in trade.Items)
                            {
                                i.Reserved = false;
                            }
                        }
                        break;
                    case "Confirm":
                        if (User.Identity.GetUserId() == trade.SendingTraderId)
                            trade.SenderConfirmed = true;
                        else
                            trade.ReceiverConfirmed = true;
                        //upon confirmation, mark all items in trade as removed
                        var itemList2 = new List<Item>();
                        foreach (Item i in trade.Items)
                        {
                            if (!i.Removed)
                            {
                                if (trade.SenderConfirmed && trade.ReceiverConfirmed)
                                {
                                    i.Removed = true;
                                }
                                    
                                itemList2.Add(i);
                            }
                        }
                        trade.Items = itemList2;
                       
                        break;
                    case "Reject":
                        trade.Rejected = true;
                        break;
                    default:
                        int rating = 0;
                        if (int.TryParse(status, out rating))
                        {
                            if (User.Identity.GetUserId() == trade.SendingTraderId)
                            {
                                trade.SenderRating = rating;
                            }
                            else
                            {
                                trade.ReceiverRating = rating;
                            }
                        }
                        break;
                }
                Trade updatedTrade = new Trade(trade);
                //update date last modified
                updatedTrade.LastModified = DateTime.Now;
                //update in database
                //also update many-to-many relationship with items
                //remove
                db.Trades.Remove(trade);
                db.SaveChanges();
                //then add back in, with new list of items
                db.Trades.Add(updatedTrade);
                db.SaveChanges();
            }

            return RedirectToAction("Pending");
        }

        [HttpGet]
        public ActionResult Pending()
        {
            var list = (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                        where (User.Identity.Name == t.ReceivingTrader.UserName
                        || User.Identity.Name == t.SendingTrader.UserName)
                        orderby t.LastModified descending
                        select t).ToList();
            //remove old canceled/rejected/completed trades from pending trades
            var recentList = new List<Trade>();
            foreach (Trade t in list)
            {
                if (!(t.DaysOld > 7 && (t.Canceled || t.Rejected || (t.SenderRating > 0 && t.ReceiverRating > 0))))
                {
                    recentList.Add(t);
                }
            }
            return View(recentList);
        }

        [HttpGet]
        public ActionResult History()
        {
            var list = (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                        where (User.Identity.Name == t.ReceivingTrader.UserName
                        || User.Identity.Name == t.SendingTrader.UserName)
                        && (t.Canceled || t.Rejected || (t.SenderRating > 0 && t.ReceiverRating > 0))
                        orderby t.LastModified descending
                        select t).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Ratings()
        {
            var list = (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                        where (User.Identity.Name == t.ReceivingTrader.UserName && t.SenderRating > 0)
                        || (User.Identity.Name == t.SendingTrader.UserName && t.ReceiverRating > 0)
                        orderby t.LastModified descending
                        select t).ToList();
            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ListAll()
        {
            var list = (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                        orderby t.LastModified descending
                        select t).ToList();
            return View(list);
        }
    }
}