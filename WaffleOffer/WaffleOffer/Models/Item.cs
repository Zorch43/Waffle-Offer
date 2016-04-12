using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Item
    {
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
    }
}