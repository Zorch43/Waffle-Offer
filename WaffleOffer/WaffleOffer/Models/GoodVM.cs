using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class GoodVM
    {
        List<ItemTag> tags = new List<ItemTag>();
        List<ItemImage> images = new List<ItemImage>();

        //  key
        public int GoodID { get; set; }
        //  What the measurement is in (pounds, pairs, etc.)
        public string Units { get; set; }
        //  number of Units offered
        public double Quantity { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } set { tags = value; } }
        public List<ItemImage> Images { get { return images; } set { images = value; } }
        public int Quality { get; set; }
    }
}