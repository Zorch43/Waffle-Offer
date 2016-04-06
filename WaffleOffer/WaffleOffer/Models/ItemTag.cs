using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class ItemTag
    {
        //  key
        [Key]
        public int TagID { get; set; }
        public string Tag { get; set; }
    }
}