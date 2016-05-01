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

                return View(model);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "SenderId,ReceiverId,Items")] TradeCreator trade)
        {
            

            //create Trade
            var model = new Trade()
            {
                SendingTraderId = trade.SenderId,
                ReceivingTraderId = trade.ReceiverId
            };

            var items = new List<Item>();

            //get list of items
            for (int i = 0; i < trade.Items.Count; i++)
            {
                items.Add(db.Items.Find(trade.Items.ElementAt(i)));
            }

            //add traded items to trade
            model.Items = items;

            //add to database
            db.Trades.Add(model);
            db.SaveChanges();


            //go back to items
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            //TODO: load list
            var list = (from t in db.Trades.Include("SendingTrader")
                        .Include("ReceivingTrader").Include("Items")
                        select t).ToList();            
            
            return View(list);
        }
    }
}