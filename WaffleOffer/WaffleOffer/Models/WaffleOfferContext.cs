using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WaffleOffer.Models
{
    public class WaffleOfferContext : IdentityDbContext<AppUser>
    {
        public WaffleOfferContext() : base("name=WaffleOfferContext")
        {
            //Configuration.LazyLoadingEnabled = false;
        }

        public System.Data.Entity.DbSet<Thread> Threads { get; set; }
        public System.Data.Entity.DbSet<Item> Items { get; set; }
        public System.Data.Entity.DbSet<Message> Messages { get; set; }
        public System.Data.Entity.DbSet<Trade> Trades { get; set; }
        public System.Data.Entity.DbSet<ItemImage> ItemImages { get; set; }
    }
}