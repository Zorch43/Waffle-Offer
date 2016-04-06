using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class Service : Item
    {
        //  key
        public int ServiceID { get; set; }
        //  number of minutes offered/desired
        public int Duration { get; set; }
    }
}