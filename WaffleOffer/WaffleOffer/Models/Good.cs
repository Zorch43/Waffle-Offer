using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class Good : Item
    {
        //  key
        public int GoodID { get; set; }
        //  What the measurement is in (pounds, pairs, etc.)
        public string Units { get; set; }
        //  number of Units offered
        public double Quantity { get; set; }
    }
}