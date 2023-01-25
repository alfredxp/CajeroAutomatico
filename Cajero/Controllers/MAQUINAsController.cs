using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cajero.Models;
using Cajero.ViewModel;

namespace Cajero.Controllers
{
    public class MAQUINAsController : Controller
    {
        private ATMDBEntities db = new ATMDBEntities();

        // GET: MAQUINAs
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index","Home");
            var mAQUINAs = db.MAQUINAs.Include(m => m.ADMINISTRADOR);
            return View(mAQUINAs.Where(m=>m.ESTATUS != "I").ToList());
        }

        // GET: MAQUINAs/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAQUINA mAQUINA = db.MAQUINAs.Find(id);
            if (mAQUINA == null)
            {
                return HttpNotFound();
            }
            return View(mAQUINA);
        }

        // GET: MAQUINAs/Create
        public ActionResult Create()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            ViewBag.idAdmin = new SelectList(db.ADMINISTRADORs, "USER_ADMIN_CD", "USER_ADMIN_NM");
            return View();
        }

        // POST: MAQUINAs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idATM,direccion,monedaDetalles")] Maquina mAQUINA)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                MAQUINA Maq = new MAQUINA();
                Maq.ADDRESS_DESC = mAQUINA.direccion;
                Maq.ID_ATM_CD = mAQUINA.idATM;
                Maq.USER_ADMIN_CD = Convert.ToInt32(Session["UserID"]);

                Maq.DETALLE_MONEDA = new List<DETALLE_MONEDA>();
                int balanceCalc = 0;
                if (mAQUINA.monedaDetalles != null)
                {
                    
                    foreach(var liMaq in mAQUINA.monedaDetalles)
                    {
                        var ma = new DETALLE_MONEDA();
                        ma.BILLETE = liMaq.Billete;
                        ma.CANTIDAD = liMaq.Cantidad;
                        Maq.DETALLE_MONEDA.Add(ma);
                        balanceCalc = balanceCalc + (ma.BILLETE * ma.CANTIDAD);
                    }
                }
                Maq.TYPE_DISTINC_CASH_NM = "";
                Maq.BALANCE_AMT = balanceCalc;
                Maq.ESTATUS = "A";
                db.MAQUINAs.Add(Maq);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.USER_ADMIN_CD = new SelectList(db.ADMINISTRADORs, "USER_ADMIN_CD", "USER_ADMIN_NM", mAQUINA.idAdmin);
            return View(mAQUINA);
        }

        // GET: MAQUINAs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAQUINA mAQUINA = db.MAQUINAs.Find(id);
            if (mAQUINA == null)
            {
                return HttpNotFound();
            }
            ViewBag.USER_ADMIN_CD = new SelectList(db.ADMINISTRADORs, "USER_ADMIN_CD", "USER_ADMIN_NM", mAQUINA.USER_ADMIN_CD);
            var maq = new ViewModel.Maquina();
            maq.idATM = mAQUINA.ID_ATM_CD;
            maq.cantidad = mAQUINA.CASH_CANT_CNT;
            maq.balance = mAQUINA.BALANCE_AMT;
            maq.direccion= mAQUINA.ADDRESS_DESC;
            maq.idAdmin= mAQUINA.USER_ADMIN_CD;
            maq.monedaDetalles = new List<Maquina.MonedaDetalle>();
           var detalle = new Maquina.MonedaDetalle();
            foreach (var dm in mAQUINA.DETALLE_MONEDA) {
                detalle.Billete = dm.BILLETE;
                detalle.Cantidad = dm.CANTIDAD;
                detalle.idATM = dm.ID_MAQUINA;
                detalle.idMonedaDetalle = dm.ID_MONEDA_DETALLE;
                maq.monedaDetalles.Add(detalle);
            }
            return View(maq);
        }

        // POST: MAQUINAs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_ATM_CD,USER_ADMIN_CD,ADDRESS_DESC,TYPE_DISTINC_CASH_NM,CASH_CANT_CNT,BALANCE_AMT")] MAQUINA mAQUINA)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(mAQUINA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.USER_ADMIN_CD = new SelectList(db.ADMINISTRADORs, "USER_ADMIN_CD", "USER_ADMIN_NM", mAQUINA.USER_ADMIN_CD);
            return View(mAQUINA);
        }

        // GET: MAQUINAs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAQUINA mAQUINA = db.MAQUINAs.Find(id);
            if (mAQUINA == null)
            {
                return HttpNotFound();
            }
            return View(mAQUINA);
        }

        // POST: MAQUINAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index", "Home");
            MAQUINA mAQUINA = db.MAQUINAs.Find(id);
            mAQUINA.ESTATUS = "I";
            db.Entry(mAQUINA).State = EntityState.Modified;
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
