using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WaffleOffer.Models
{
    public class WaffleOfferDBContext : IdentityDbContext<AppUser>
    {
        public WaffleOfferDBContext() : base("name=WaffleOfferDBContext")
        {
        }

        public System.Data.Entity.DbSet<WaffleOffer.Models.Item> Items { get; set; }

        public System.Data.Entity.DbSet<WaffleOffer.Models.ItemImage> ItemImages { get; set; }

       //public System.Data.Entity.DbSet<WaffleOffer.Models.Trader> Traders { get; set; }
    }
}