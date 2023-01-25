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
    public class ADMINISTRADORsController : Controller
    {
        private ATMDBEntities db = new ATMDBEntities();

        public ActionResult Admin()
        {
            if (Session["UserName"] != null)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ADMINISTRADOR objUser)
        {
            if (ModelState.IsValid)
            {
                using (ATMDBEntities db = new ATMDBEntities())
                {
                    var obj = db.ADMINISTRADORs.Where(a => a.USER_ADMIN_NM.Equals(objUser.USER_ADMIN_NM) && a.ADMIN_PASSWORD_CD.Equals(objUser.ADMIN_PASSWORD_CD)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.USER_ADMIN_CD.ToString();
                        Session["UserName"] = obj.USER_ADMIN_NM.ToString();
                        return RedirectToAction("Admin");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult Logout()
        {
            if (Session["UserName"]!= null)
            {
                Session["UserID"] = null;
                Session["UserName"] = null;


            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: ADMINISTRADORs
        public ActionResult Index()
        {
            return View(db.ADMINISTRADORs.ToList());
        }

        // GET: ADMINISTRADORs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ADMINISTRADOR aDMINISTRADOR = db.ADMINISTRADORs.Find(id);
            if (aDMINISTRADOR == null)
            {
                return HttpNotFound();
            }
            return View(aDMINISTRADOR);
        }

        // GET: ADMINISTRADORs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ADMINISTRADORs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "USER_ADMIN_CD,USER_ADMIN_NM,ADMIN_PASSWORD_CD")] ADMINISTRADOR aDMINISTRADOR)
        {
            if (ModelState.IsValid)
            {
                db.ADMINISTRADORs.Add(aDMINISTRADOR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aDMINISTRADOR);
        }

        // GET: ADMINISTRADORs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ADMINISTRADOR aDMINISTRADOR = db.ADMINISTRADORs.Find(id);
            if (aDMINISTRADOR == null)
            {
                return HttpNotFound();
            }
            return View(aDMINISTRADOR);
        }

        // POST: ADMINISTRADORs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "USER_ADMIN_CD,USER_ADMIN_NM,ADMIN_PASSWORD_CD")] ADMINISTRADOR aDMINISTRADOR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aDMINISTRADOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aDMINISTRADOR);
        }

        // GET: ADMINISTRADORs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ADMINISTRADOR aDMINISTRADOR = db.ADMINISTRADORs.Find(id);
            if (aDMINISTRADOR == null)
            {
                return HttpNotFound();
            }
            return View(aDMINISTRADOR);
        }

        // POST: ADMINISTRADORs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ADMINISTRADOR aDMINISTRADOR = db.ADMINISTRADORs.Find(id);
            db.ADMINISTRADORs.Remove(aDMINISTRADOR);
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
