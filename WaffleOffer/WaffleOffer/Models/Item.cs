using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Item
    {

        public enum ItemType { Want, Have }
        List<ItemTag> tags = new List<ItemTag>(); // removed extra 'new'
        List<ItemImage> images = new List<ItemImage>();

        [Key]
        public int ItemID { get; set; }
        [Required]
        public string Name { get; set; } // replace with title
        [Required]
        public string Description { get; set; } //  description from api
        public List<ItemTag> Tags { get { return tags; } set { tags = value; } } // if they come from api, maybe change to categories
        public List<ItemImage> Images { get { return images; } set { images = value; } }
        [Required]
        public int Quality { get; set; }
        //  What the Item is measured in (minutes, pounds, etc)
        [Required]
        public string Units { get; set; } // don't need
        //  number of Units offered
        [Required]
        public double Quantity { get; set; } // don't need
        //type of listing
        public ItemType ListingType { get; set; }
        //listing user
        public string ListingUser { get; set; }

        //list of trades that the item appears/has appeared in
        //mostly to satisfy EF, but might come in handy
        public List<Trade> Trades { get; set; }

        //trade statuses - Haves only
        //Reserved: Reserved Items are part of a Trade that has been accepted.
        //Reserved Items are visible to the Trader (and Admin) in Haves
        //Reserved Items only appear in the trades they are already in.
        //If a Reserved Item is in a trade that is not accepted, it is displayed with a note.
        public bool Reserved { get; set; }

        //Removed:
        //Removed Items are not visible anywhere but completed (canceled/rejected/confirmed) trades
        //If an item is Removed and does not appear in any trades, it is deleted from the database
        public bool Removed { get; set; }

        public override string ToString()
        {
            if (Reserved)
                return Name + " (Reserved)";
            else if (Removed)
                return Name + " (Removed)";
            else
                return Name;
        }
    }
}