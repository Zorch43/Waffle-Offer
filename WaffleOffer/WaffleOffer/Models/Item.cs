using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Item
    {

        public enum ItemType { Want, Have}
        List<ItemTag> tags = new List<ItemTag>(); // removed extra 'new'
        List<ItemImage> images = new List<ItemImage>();

        [Key]
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } set { tags = value; } }
        public List<ItemImage> Images { get { return images; } set { images = value; } }
        public int Quality { get; set; }
        //  What the Item is measured in (minutes, pounds, etc)
        public string Units { get; set; }
        //  number of Units offered
        public double Quantity { get; set; }
        //type of listing
        public ItemType ListingType { get; set; }
        //listing user
        public string ListingUser { get; set; }

        //list of trades that the item appears/has appeared in
        //mostly to satisfy EF, but might come in handy
        public List<Trade> Trades { get; set; }
    }
}