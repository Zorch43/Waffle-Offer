using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class Item
    {
        List<ItemTag> tags = new List<ItemTag>();
<<<<<<< HEAD
        List<ItemImage> images = new List<ItemImage>();
=======
>>>>>>> Trade

        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } set { tags = value; } }
        public List<ItemImage> Images { get { return images; } set { images = value; } }
        public int Quality { get; set; }

    }
}