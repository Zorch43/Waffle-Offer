using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Trade
    {
        // List of items to trade
        List<Item> items = new List<Item>();

        // Item offered in trade
        public string Offer { get; set; }

        // Item wanted in trade
        public string Wanted { get; set; }

        // Trade accepted
        public bool Accepted { get; set; }

        // Trade declined
        public bool Decline { get; set; }
    }
}