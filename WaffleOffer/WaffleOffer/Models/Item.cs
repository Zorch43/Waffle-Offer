using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Item
    {
        List<ItemTag> tags = new List<ItemTag>();
        List<ItemImage> images = new List<ItemImage>();

        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } set; }
        public List<ItemImage> Images { get { return images; } set; }
        public int Quality { get; set; }

    }
}