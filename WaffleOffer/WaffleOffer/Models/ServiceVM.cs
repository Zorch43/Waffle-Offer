using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class ServiceVM
    {
        List<ItemTag> tags = new List<ItemTag>();
        List<ItemImage> images = new List<ItemImage>();

        //  key
        public int ServiceID { get; set; }
        //  number of minutes offered/desired
        public int Duration { get; set; }


        public string Name { get; set; }
        public string Description { get; set; }
        public List<ItemTag> Tags { get { return tags; } set { tags = value; } }
        public List<ItemImage> Images { get { return images; } set { images = value; } }
        public int Quality { get; set; }
    }
}