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
        public int? TradeId { get; set; }
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

        [NotMapped]
        public List<Item> SenderHaves
        {
            get
            {
                var list = new List<Item>();
                foreach (Item i in SendingTrader.TraderAccount.Haves)
                {
                    bool found = false;
                    foreach (Item m in Items)
                    {
                        if (i.ItemID == m.ItemID)
                        {
                            found = true;
                            break;
                        }  
                    }

                    if (!found)
                    {
                        list.Add(i);
                    }
                }

                return list;
            }
        }

        [NotMapped]
        public List<Item> ReceiverHaves
        {
            get
            {
                var list = new List<Item>();
                foreach (Item i in ReceivingTrader.TraderAccount.Haves)
                {
                    bool found = false;
                    foreach (Item m in Items)
                    {
                        if (i.ItemID == m.ItemID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        list.Add(i);
                    }
                }

                return list;
            }
        }

        //whether trade has been submitted
        public bool Submitted { get; set; }


        //whether the sending player has opted to cancel the trade
        public bool Canceled { get; set; }

        //list of previous item ids
        //for reverting to previous trade without screwing up the database
        public List<int> PrevItemIds { get; set; }

        //Whether the receiver has rejected the offer
        public bool Rejected { get; set; }

        //whether the receiver has accepted the offer
        public bool Accepted { get; set; }

        //whether the sender has confirmed that the trade has gone through
        public bool SenderConfirmed { get; set; }

        //whether the receiver has confirmed that the trade has gone through
        public bool ReceiverConfirmed { get; set; }

        //sender's rating of the completed trade
        public int SenderRating { get; set; }

        //receiver's rating of the completed trade
        public int ReceiverRating { get; set; }

        public string GetTradeStatusMessage(bool trader)
        {
            if (Canceled)
                return "Canceled";
            else if (Rejected)
                return "Rejected";
            else if (SenderConfirmed && ReceiverConfirmed)
                return "Confirmed";
            else if ((trader && SenderConfirmed) || (!trader && ReceiverConfirmed))
                return "Confirmation Pending";
            else if ((trader && SenderRating > 0) || (!trader && ReceiverRating > 0))
                return "Rating Pending";
            else if (SenderRating > 0 && ReceiverRating > 0)
                return "Completed";
            else if (Accepted)
                return "Accepted";
            else if (Submitted)
            {
                if (trader)
                    return "Trade Submitted";
                else
                    return "Trade Received";
            }
            else
                return "New Trade";   
        }
    }
}