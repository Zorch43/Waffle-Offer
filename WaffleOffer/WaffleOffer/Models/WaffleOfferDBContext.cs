using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class WaffleOfferDBContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public WaffleOfferDBContext() : base("name=WaffleOfferDBContext")
        {
        }

        public System.Data.Entity.DbSet<WaffleOffer.Models.Good> Goods { get; set; }

        public System.Data.Entity.DbSet<WaffleOffer.Models.Service> Services { get; set; }
    
    }
}
