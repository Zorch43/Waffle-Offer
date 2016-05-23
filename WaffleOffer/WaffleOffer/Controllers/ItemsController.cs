using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;
using Microsoft.AspNet.Identity;

namespace WaffleOffer.Controllers
{
    public class ItemsController : Controller
    {
        private WaffleOfferContext db = new WaffleOfferContext();


        // GET: /Items/
        //public ActionResult Index(string sortOdr, string searchStg)  // sorting and simple search
        public ActionResult Index(string sortOdr, string searchStg, string itemType)  // sorting and filtered search
        {
            // Viewbags for sorting
            ViewBag.NameSort = String.IsNullOrEmpty(sortOdr) ? "name_desc" : "";
            ViewBag.QualitySort = sortOdr == "Quality" ? "quality_desc" : "Quality";

            // Search with filters for Wants or Haves
            // NOTE: Using tutorial "Search" from asp.net (http://www.asp.net/mvc/overview/getting-started/introduction/adding-search)
            var TypeLst = new List<string>();

            string Have = Item.ItemType.Have.ToString();
            string Want = Item.ItemType.Want.ToString();

            var TypeQry = from t in db.Items
                          orderby t.ListingType  // (ListingType = 1) is Have
                          select t.ListingType;

            string[] TypeOpt = { Have, Want };

            /*string TypeOpt = "0";

            if (TypeOpt == "0")
            {
                //not sure about this
            }*/

            TypeLst.AddRange(TypeOpt);
            //TypeLst.AddRange(TypeQry.Distinct());  // doesn't seem to want to allow this. Maybe because of enum?
            ViewBag.itemType = new SelectList(TypeLst);

            var items = from i in db.Items
                        select i;

            if (!String.IsNullOrEmpty(searchStg))
            {
                items = items.Where(s => s.Name.Contains(searchStg) || s.Description.Contains(searchStg));
            }

            // for selecting Want or Have
            if (!String.IsNullOrEmpty(itemType))
            {
                items = items.Where(t => t.ListingType.ToString() == itemType);  // suggested by Timo
                //items = items.Where(t => t.ItemType == itemType);
                //items = items.Where(t => t.ItemType.Have == itemType|| t.ItemType.Want == itemType)); 
                //items = items.Where(f => f.ListingType == itemType);
            } /**/


            // Simple Search
            /*var items = from i in db.Items
                        select i;

            if (!String.IsNullOrEmpty(searchStg))
            {
                items = items.Where(s => s.Name.Contains(searchStg) || s.Description.Contains(searchStg));
                //items = items.Where(s => s.Name.Contains(searchStg));  // Does single word/ words in same order search in Name only
                //items = items.Where(s => s.Name.Contains(searchStg) || s.Name.Contains(searchStg2)); // tried a second search criteria with string. Did not work.
            } */


            // sorting by Name and Quality
            switch (sortOdr)
            {
                case "name_desc":
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case "Quality":
                    items = items.OrderBy(i => i.Quality);
                    break;
                case "quality_desc":
                    items = items.OrderByDescending(i => i.Quality);
                    break;
                default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }

            return View(items.ToList());

            // Default list of items
            //return View(db.Items.ToList());
        }

        public ActionResult Items(string userName, Item.ItemType type)
        {
            ViewBag.TypeHeading = type + "s";
            ViewBag.ListingUser = userName;

            var items = (from i in db.Items
                        where i.ListingType == type && i.ListingUser == userName
                        select i).ToList();



            return View(items);
        }

        // GET: /Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Item item = db.Items.Find(id);
            Item item = db.Items.Include(i => i.Images).SingleOrDefault(i => i.ItemID == id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: /Items/Create
        [HttpGet]
        public ActionResult Create(string type)
        {
            //// Default action
            //return View();
            
            if (type == Item.ItemType.Have.ToString() || type == Item.ItemType.Want.ToString())
            {
                return View(new Item()
                {
                    ListingType = (Item.ItemType)Enum.Parse(typeof(Item.ItemType), type),
                    ListingUser = User.Identity.Name
                });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }

        // POST: /Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,Name,Description,Quality,Units,Quantity,ListingType,ListingUser")] Item item, HttpPostedFileBase upload)
        {
            var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (!validImageTypes.Contains(upload.ContentType))
                    {
                        ModelState.AddModelError("Images", "Please choose either a GIF, JPG or PNG image.");
                    }
                    else 
                    { 
                        var image = new ItemImage
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            image.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        item.Images.Add(image);
                    }
                }


                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ItemID,Name,Description,Quality,Units,Quantity,ListingType,ListingUser")] Item item) // from exisiting fields in the Create view
        ////public ActionResult Create([Bind(Include="ItemID,Name,Description,Quality,Units,Quantity,ListingType,ListingUser")] Item item)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Items.Add(item);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View(item);
        //    }         
        //}

      

        // GET: /Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ItemID,Name,Description,Quality,Units,Quantity")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: /Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}