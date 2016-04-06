using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Item
    {
        List<ItemTag> tags = new List<ItemTag>(); // removed extra 'new'
        //new List<ItemTag> tags = new List<ItemTag>();

        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } } // issues with 'set'
        //public List<ItemTag> Tags { get { return tags; } set; }
        public int Quality { get; set; }

    }
}