using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class AppUser : IdentityUser
    {

        //non-editable
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //editable
        //location
        public string City { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }//required

        //profile
        public string ProfileText { get; set; }

        ////items
        //public List<Item> Wants { get; set; }
        //public List<Item> Haves { get; set; }
        //public List<Trade> Trades { get; set; }

        ////social
        //public List<Trader> FavoriteTraders { get; set; }

    }
}