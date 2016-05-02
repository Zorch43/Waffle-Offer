using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class TradeCreator
    {
        //Trade id (nullable)
        //null if new trade, set to a trade id if a counter-offer
        public int? TradeId { get; set; }
        //Trader ids
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        //list of item ids
        public ICollection<int> Items{get; set;}
    }
}