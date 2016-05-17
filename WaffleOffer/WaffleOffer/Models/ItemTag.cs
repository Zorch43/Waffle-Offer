using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class ItemTag
    {
        [Key]
        public int TagID { get; set; }
        public int ItemID { get; set; }
        public string Tag { get; set; }
    }
}