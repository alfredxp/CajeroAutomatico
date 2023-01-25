using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cajero.Models;

namespace Cajero.Controllers
{
    public class DETALLE_MONEDAController : Controller
    {
        private ATMDBEntities db = new ATMDBEntities();

        // GET: DETALLE_MONEDA
        public ActionResult Index()
        {
            var dETALLE_MONEDA = db.DETALLE_MONEDA.Include(d => d.MAQUINA);
            return View(dETALLE_MONEDA.ToList());
        }

        // GET: DETALLE_MONEDA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DETALLE_MONEDA dETALLE_MONEDA = db.DETALLE_MONEDA.Find(id);
            if (dETALLE_MONEDA == null)
            {
                return HttpNotFound();
            }
            return View(dETALLE_MONEDA);
        }

        // GET: DETALLE_MONEDA/Create
        public ActionResult Create()
        {
            ViewBag.ID_MAQUINA = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC");
            return View();
        }

        // POST: DETALLE_MONEDA/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_MONEDA_DETALLE,BILLETE,CANTIDAD,ID_MAQUINA")] DETALLE_MONEDA dETALLE_MONEDA)
        {
            if (ModelState.IsValid)
            {
                db.DETALLE_MONEDA.Add(dETALLE_MONEDA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_MAQUINA = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC", dETALLE_MONEDA.ID_MAQUINA);
            return View(dETALLE_MONEDA);
        }

        // GET: DETALLE_MONEDA/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DETALLE_MONEDA dETALLE_MONEDA = db.DETALLE_MONEDA.Find(id);
            if (dETALLE_MONEDA == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_MAQUINA = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC", dETALLE_MONEDA.ID_MAQUINA);
            return View(dETALLE_MONEDA);
        }

        // POST: DETALLE_MONEDA/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_MONEDA_DETALLE,BILLETE,CANTIDAD,ID_MAQUINA")] DETALLE_MONEDA dETALLE_MONEDA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dETALLE_MONEDA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_MAQUINA = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC", dETALLE_MONEDA.ID_MAQUINA);
            return View(dETALLE_MONEDA);
        }

        // GET: DETALLE_MONEDA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DETALLE_MONEDA dETALLE_MONEDA = db.DETALLE_MONEDA.Find(id);
            if (dETALLE_MONEDA == null)
            {
                return HttpNotFound();
            }
            return View(dETALLE_MONEDA);
        }

        // POST: DETALLE_MONEDA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DETALLE_MONEDA dETALLE_MONEDA = db.DETALLE_MONEDA.Find(id);
            db.DETALLE_MONEDA.Remove(dETALLE_MONEDA);
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
