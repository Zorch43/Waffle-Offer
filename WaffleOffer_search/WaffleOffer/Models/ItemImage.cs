using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        //  a byte array of the image to store in the database
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}