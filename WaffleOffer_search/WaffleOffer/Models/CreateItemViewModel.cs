using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class CreateItemViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quality { get; set; }
        //  What the Item is measured in (minutes, pounds, etc)
        public string Units { get; set; }
        //  number of Units offered
        public double Quantity { get; set; }
        //want or have
        public string ItemType { get; set; }
    }
}