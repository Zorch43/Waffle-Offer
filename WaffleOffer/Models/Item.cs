﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Item
    {
        List<ItemTag> tags = new List<ItemTag>();

        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } }  // had issues with the setter
        //public List<ItemTag> Tags { get { return tags; } set; }
        public int Quality { get; set; }

    }
}