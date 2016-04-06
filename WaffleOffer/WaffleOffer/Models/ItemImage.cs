using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class ItemImage
    {
        //  key
        [Key]
        public int ImageID { get; set; }
        
        public int ItemID { get; set; }
        //  may or may not want captions for each photo
        public string Caption { get; set; }
        //  filename for the image uploaded
        public string FileName { get; set; }
    }
}