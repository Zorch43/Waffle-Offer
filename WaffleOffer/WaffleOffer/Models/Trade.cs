using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        //recieving trader
        public string ReceivingTraderId { get; set; }
        public AppUser ReceivingTrader { get; set; }

        //all items from both sides
        //because separating them makes the database weird
        public List<Item> Items { get; set; }

        //items belonging to the sending trader
        [NotMapped]
        public List<Item> SendingItems
        {
            get
            {
                var sItems = new List<Item>();
                foreach (Item i in Items)
                {
                    if (i.ListingUser == SendingTrader.UserName)
                        sItems.Add(i);
                }

                return sItems;
            }
        }

        //items belonging to the receiving trader
        [NotMapped]
        public List<Item> ReceivingItems
        {
            get
            {
                var rItems = new List<Item>();
                foreach (Item i in Items)
                {
                    if (i.ListingUser == ReceivingTrader.UserName)
                        rItems.Add(i);
                }

                return rItems;
            }
        }

    }
}