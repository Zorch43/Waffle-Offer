using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class ItemsController : Controller
    {
        private WaffleOfferDBContext db = new WaffleOfferDBContext();

        // GET: /Items/
        public ActionResult Index(string sortOdr, string searchStg)
        {
            ViewBag.NameSort = String.IsNullOrEmpty(sortOdr) ? "name_desc" : "";
            ViewBag.QualitySort = sortOdr == "Quality" ? "quality_desc" : "Quality";

            var items = from i in db.Items
                        select i;

            if (!String.IsNullOrEmpty(searchStg))
            {
                items = items.Where(s => s.Name.Contains(searchStg) || s.Description.Contains(searchStg));
                //items = items.Where(s => s.Name.Contains(searchStg));  // Does single word/ words in same order search in Name only
                //items = items.Where(s => s.Name.Contains(searchStg) || s.Name.Contains(searchStg2)); // tried a second search criteria with string. Did not work.
            }

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

        // GET: /Items/Details/5
        public ActionResult Details(int? id)
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

        // GET: /Items/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ItemID,Name,Description,Quality,Units,Quantity")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

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
