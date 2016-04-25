using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class TradeController : Controller
    {
         private readonly UserManager<AppUser> userManager;

        private WaffleOfferDBContext db = new WaffleOfferDBContext();

        public TradeController() : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public TradeController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        // GET: Trade
        public ActionResult Index(string partner)
        {
            //find partner
            var tradePartner = userManager.FindByName(partner);
            if (tradePartner != null)
            {
                var model = new Trade()
                {
                    SendingTrader = userManager.FindByName(User.Identity.Name),
                    SendingItems = new List<Item>(),
                    ReceivingTrader = tradePartner,
                    ReceivingItems = new List<Item>()
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

                return View(model);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "SenderId,ReceiverId,OfferedItems,RequestedItems")] TradeCreator trade)
        {
            var offered = new List<Item>();
            var requested = new List<Item>();

            //get list of offered items
            for (int i = 0; i < trade.OfferedItems.Count; i++)
            {
                offered.Add(db.Items.Find(trade.OfferedItems.ElementAt(i)));
            }

            
            //get list of requested items
            for (int i = 0; i < trade.RequestedItems.Count; i++)
            {
                requested.Add(db.Items.Find(trade.RequestedItems.ElementAt(i)));
            }

            //create Trade
            var model = new Trade()
            {
                SendingItems = offered,
                ReceivingItems = requested,
                SendingTraderId = trade.SenderId,
                ReceivingTraderId = trade.ReceiverId
            };
            //add to database
            db.Trades.Add(model);
            db.SaveChanges();


            //go back to items
            return RedirectToAction("index", "items");
        }
    }
}