using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class TradeCreator
    {
        //Trader ids
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        //list of item ids
        public ICollection<int> OfferedItems{get; set;}
        public ICollection<int> RequestedItems { get; set; }
    }
}