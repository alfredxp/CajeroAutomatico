using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cajero.Models;

namespace Cajero.Controllers
{
    public class TransactionController : Controller
    {
        private ATMDBEntities db = new ATMDBEntities();

        // GET: Transaction
        public ActionResult Index()
        {
            var aTM_TRANSACTION = db.ATM_TRANSACTION.Include(a => a.USUARIO).Include(a => a.MAQUINA).OrderBy(s=>s.MAQUINA.ID_ATM_CD);
            return View(aTM_TRANSACTION.ToList());
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            string desdestr = form["datetimepicker1"];
            string hastastr = form["datetimepicker2"];

            var desde = Convert.ToDateTime(desdestr);
            var hasta = Convert.ToDateTime(hastastr);
            var aTM_TRANSACTION = db.ATM_TRANSACTION.Include(a => a.USUARIO).Include(a => a.MAQUINA).OrderBy(s => s.MAQUINA.ID_ATM_CD)
                .Where(t=>t.TRANSACTION_DT >= desde && t.TRANSACTION_DT <= hasta);
            return View(aTM_TRANSACTION.ToList());
        }

        // GET: Transaction/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATM_TRANSACTION aTM_TRANSACTION = db.ATM_TRANSACTION.AsQueryable().Where(t=>t.TRANS_NO == id).FirstOrDefault();
            if (aTM_TRANSACTION == null)
            {
                return HttpNotFound();
            }
            return View(aTM_TRANSACTION);
        }

        // GET: Transaction/Create
        public ActionResult Create()
        {
            ViewBag.Card_Number_ID = new SelectList(db.USUARIOs, "Card_Number_ID", "CEDULA_CD");
            ViewBag.ID_ATM_CD = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC");
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Card_Number_ID,TRANS_NO,TRANSACTION_DT,ID_ATM_CD,TRANSACTION_TYPE_NM,TRANSACTION_AMT")] ATM_TRANSACTION aTM_TRANSACTION)
        {
            if (ModelState.IsValid)
            {
                db.ATM_TRANSACTION.Add(aTM_TRANSACTION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Card_Number_ID = new SelectList(db.USUARIOs, "Card_Number_ID", "CEDULA_CD", aTM_TRANSACTION.Card_Number_ID);
            ViewBag.ID_ATM_CD = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC", aTM_TRANSACTION.ID_ATM_CD);
            return View(aTM_TRANSACTION);
        }

        // GET: Transaction/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATM_TRANSACTION aTM_TRANSACTION = db.ATM_TRANSACTION.Find(id);
            if (aTM_TRANSACTION == null)
            {
                return HttpNotFound();
            }
            ViewBag.Card_Number_ID = new SelectList(db.USUARIOs, "Card_Number_ID", "CEDULA_CD", aTM_TRANSACTION.Card_Number_ID);
            ViewBag.ID_ATM_CD = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC", aTM_TRANSACTION.ID_ATM_CD);
            return View(aTM_TRANSACTION);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Card_Number_ID,TRANS_NO,TRANSACTION_DT,ID_ATM_CD,TRANSACTION_TYPE_NM,TRANSACTION_AMT")] ATM_TRANSACTION aTM_TRANSACTION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aTM_TRANSACTION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Card_Number_ID = new SelectList(db.USUARIOs, "Card_Number_ID", "CEDULA_CD", aTM_TRANSACTION.Card_Number_ID);
            ViewBag.ID_ATM_CD = new SelectList(db.MAQUINAs, "ID_ATM_CD", "ADDRESS_DESC", aTM_TRANSACTION.ID_ATM_CD);
            return View(aTM_TRANSACTION);
        }

        // GET: Transaction/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATM_TRANSACTION aTM_TRANSACTION = db.ATM_TRANSACTION.Find(id);
            if (aTM_TRANSACTION == null)
            {
                return HttpNotFound();
            }
            return View(aTM_TRANSACTION);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ATM_TRANSACTION aTM_TRANSACTION = db.ATM_TRANSACTION.Find(id);
            db.ATM_TRANSACTION.Remove(aTM_TRANSACTION);
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

        public ActionResult ConfirmTrx(string id)
        {
            
            ViewBag.IdTrx = id;
            return View();
        }
    }
}
