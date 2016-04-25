using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Trade
    {

        //key
        [Key]
        public int TradeId { get; set; }

        //sending trader
        public string SendingTraderId { get; set; }
        public AppUser SendingTrader { get; set; }
        public List<Item> SendingItems { get; set; }

        //recieving trader
        public string ReceivingTraderId { get; set; }
        public AppUser ReceivingTrader { get; set; }
        public List<Item> ReceivingItems { get; set; }

    }
}