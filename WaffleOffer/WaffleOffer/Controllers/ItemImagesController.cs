using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class ItemImagesController : Controller
    {
        private WaffleOfferContext db = new WaffleOfferContext();
        //
        // GET: /File/
        public ActionResult Index(int id)
        {
            var imageToRetrieve = db.ItemImages.Find(id);
            return File(imageToRetrieve.Content, imageToRetrieve.ContentType);
        }
	}
}