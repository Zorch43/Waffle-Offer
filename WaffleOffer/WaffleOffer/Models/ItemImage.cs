using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class ItemImage
    {
        //  key
        public int ImageID { get; set; }
        //  key to match the image to the item
        public int ItemID { get; set; }
        //  may or may not want captions for each photo
        public string Caption { get; set; }
        //  filename for the image uploaded
        public string FileName { get; set; }
    }
}