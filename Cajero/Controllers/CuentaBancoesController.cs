using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cajero.Models;
using Cajero.Utils;
using Cajero.ViewModel;

namespace Cajero.Controllers
{
    public class CuentaBancoesController : Controller
    {
        private ATMDBEntities db = new ATMDBEntities();

        // GET: CuentaBancoes
        public ActionResult Opciones(string NoCuenta, string PIN)
        {
            CuentaBancoViewModel cb;
            int numeroCuenta = Convert.ToInt32(NoCuenta.Decrypt());
            USUARIO item = db.USUARIOs.Find(numeroCuenta);
            if(item == null)
            {
                TempData["NOTFOUND"] = "true";
                return RedirectToAction("Index");
            }
            if (item.USER_PIN_CD != Convert.ToInt32(PIN.Decrypt()))
            {
                TempData["PINWRONG"] = "true";
                return RedirectToAction("Index");
            }
            cb = new ViewModel.CuentaBancoViewModel();
            cb.Balance = item.BALANCE_USER_CURRENT_AMT;
            cb.PIN = item.USER_PIN_CD.ToString();
            cb.NoCuenta = item.ACCOUNT_USER_CD.ToString();
            cb.IdUser = item.CEDULA_CD;
            cb.Id = item.Card_Number_ID;
            cb.typeAccount = item.ACCOUNT_TYPE_DESC;
            return View(cb);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CuentaBancoViewModel cuentaParam)
        {
            int ct = Convert.ToInt32(cuentaParam.NoCuenta);
            int pin = Convert.ToInt32(cuentaParam.PIN);
            var cuenta = db.USUARIOs.ToList().Find(m=>m.Card_Number_ID == ct);
            if (cuenta == null)
            {
                TempData["NOTFOUND"] = "true";
                return RedirectToAction("Index");
            }
            if (cuenta.USER_PIN_CD != Convert.ToInt32(pin))
            {
                TempData["PINWRONG"] = "true";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Opciones",new { NoCuenta = cuenta.Card_Number_ID.ToString().Encrypt(), PIN = cuenta.USER_PIN_CD.ToString().Encrypt() });
        }

        // GET: CuentaBancoes/Details/5
        public ActionResult Deposito(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO cuentaBanco = db.USUARIOs.Find(id);
            CuentaBancoViewModel cuenta = new CuentaBancoViewModel();
            cuenta.PIN = cuentaBanco.USER_PIN_CD.ToString();
            cuenta.NoCuenta = cuentaBanco.ACCOUNT_USER_CD.ToString();
            cuenta.IdUser = cuentaBanco.CEDULA_CD;
            cuenta.Id = cuentaBanco.Card_Number_ID;
            cuenta.Balance = cuentaBanco.BALANCE_USER_CURRENT_AMT;
            cuenta.typeAccount = cuentaBanco.ACCOUNT_TYPE_DESC;
            cuenta.Cedula = cuentaBanco.CEDULA_CD;
            if (cuentaBanco == null)
            {
                return HttpNotFound();
            }
            return View(cuenta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposito([Bind(Include = "Id,Balance,NoCuenta,PIN,IdUser,Deposito,Retiro,Cedula,typeAccount")] CuentaBancoViewModel cuentaBanco, FormCollection collection)
        {
            USUARIO cuentaBancoTemp = db.USUARIOs.Find(cuentaBanco.Id);
            if (cuentaBanco.PIN == cuentaBancoTemp.USER_PIN_CD.ToString())
            {
                if (ModelState.IsValid)
                {
                    if (collection["options"] == "100")
                        cuentaBanco.Deposito = 100;
                    else if (collection["options"] == "200")
                        cuentaBanco.Deposito = 200;
                    else if (collection["options"] == "500")
                        cuentaBanco.Deposito = 500;
                    else if (collection["options"] == "1000")
                        cuentaBanco.Deposito = 1000;
                    else if (collection["options"] == "2000")
                        cuentaBanco.Deposito = 2000;
                    else if (cuentaBanco.Deposito > 0)
                        cuentaBanco.Deposito = cuentaBanco.Deposito;

                    cuentaBancoTemp.BALANCE_USER_CURRENT_AMT = cuentaBancoTemp.BALANCE_USER_CURRENT_AMT + cuentaBanco.Deposito;
                    cuentaBancoTemp.Card_Number_ID = cuentaBanco.Id;
                    cuentaBancoTemp.CEDULA_CD = cuentaBanco.Cedula;
                    cuentaBancoTemp.ACCOUNT_USER_CD = Convert.ToInt32(cuentaBanco.NoCuenta);
                    cuentaBancoTemp.USER_PIN_CD = Convert.ToInt32(cuentaBanco.PIN);
                    cuentaBancoTemp.ACCOUNT_TYPE_DESC = cuentaBanco.typeAccount;
                    
                    db.Entry(cuentaBancoTemp).State = EntityState.Modified;

                    var maquinas = db.MAQUINAs.ToList();
                    var rand = new Random();
                    var maquina = maquinas[rand.Next(maquinas.Count)];
                    maquina.BALANCE_AMT = maquina.BALANCE_AMT + cuentaBanco.Deposito;
                    db.Entry(maquina).State = EntityState.Modified;

                    ATM_TRANSACTION transa = new ATM_TRANSACTION();
                    transa.Card_Number_ID = cuentaBanco.Id;
                    transa.TRANS_NO = Utils.Utilidades.RandomString(16);
                    transa.TRANSACTION_DT = DateTime.Now;
                    transa.ID_ATM_CD = 1;
                    transa.MAQUINA = maquina;
                    transa.TRANSACTION_TYPE_NM = Utils.Constantes.TIPO_TRANS_DEPOSIT;
                    transa.TRANSACTION_AMT = cuentaBanco.Deposito;
                    db.ATM_TRANSACTION.Add(transa);
                    TempData["EXITOSO"] = "true";
                    db.SaveChanges();
                    return RedirectToAction("ConfirmTrx", "Transaction", new { id = transa.TRANS_NO });
                }
            }
            else
            {
                TempData["WRONGPIN"] = "true";
                return RedirectToAction("Inicio");
            }
            return View(cuentaBanco);
        }

        public ActionResult ChangePin(int Id)
        {
            USUARIO cuentaBancoTemp = db.USUARIOs.Find(Id);
            return View(cuentaBancoTemp);
        }

        [HttpPost]
        public ActionResult ChangePin(USUARIO cuenta)
        {
            if (cuenta == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int newPin = cuenta.USER_PIN_CD;
            USUARIO cuentaBanco = db.USUARIOs.Find(cuenta.Card_Number_ID);
            if (cuentaBanco == null)
            {
                return HttpNotFound();
            }
            cuentaBanco.USER_PIN_CD = newPin;
            db.Entry(cuentaBanco).State = EntityState.Modified;
            db.SaveChanges();
            TempData["CHANGEPIN"] = "true";
            return RedirectToAction("Index");
        }

        // GET: CuentaBancoes/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: CuentaBancoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Balance,NoCuenta,PIN,IdUser,Cedula,typeAccount")] CuentaBancoViewModel cuentaBanco)
        {
            if (ModelState.IsValid)
            {
                USUARIO cuenta = new USUARIO();
                cuenta.USER_PIN_CD = Convert.ToInt32(cuentaBanco.PIN);
                cuenta.ACCOUNT_USER_CD = Convert.ToInt32(cuentaBanco.NoCuenta);
                cuenta.CEDULA_CD = cuentaBanco.Cedula.ToString();
                cuenta.Card_Number_ID = Convert.ToInt32(cuentaBanco.NoCuenta);

                cuenta.ACCOUNT_TYPE_DESC = cuentaBanco.typeAccount;
                cuenta.BALANCE_USER_CURRENT_AMT = cuentaBanco.Balance;
                if(db.USUARIOs.Find(cuenta.ACCOUNT_USER_CD) != null)
                {
                    ModelState.AddModelError("", "Este numero de cuenta ya existe.");
                    return View(cuentaBanco);
                }
                USUARIO tempCuenta = db.USUARIOs.Find(cuenta.Card_Number_ID);
                if(tempCuenta != null)
                {
                    ModelState.AddModelError("", "Este numero de cuenta ya existe.");
                    return View(cuentaBanco);
                }

                db.USUARIOs.Add(cuenta);
                db.SaveChanges();

                TempData["CuentaCreada"] = cuenta.Card_Number_ID;
                return RedirectToAction("Inicio");
            }

            return View(cuentaBanco);
        }

        // GET: CuentaBancoes/Edit/5
        public ActionResult Retiro(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO cuentaBanco = db.USUARIOs.Find(id);
            CuentaBancoViewModel cuenta = new CuentaBancoViewModel();
            cuenta.PIN = cuentaBanco.USER_PIN_CD.ToString();
            cuenta.NoCuenta = cuentaBanco.ACCOUNT_USER_CD.ToString();
            cuenta.Cedula = cuentaBanco.CEDULA_CD;
            cuenta.Id = cuentaBanco.Card_Number_ID;
            cuenta.Balance = cuentaBanco.BALANCE_USER_CURRENT_AMT;
            cuenta.typeAccount = cuentaBanco.ACCOUNT_TYPE_DESC;
            if (cuentaBanco == null)
            {
                return HttpNotFound();
            }
            return View(cuenta);
        }

        // POST: CuentaBancoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Retiro([Bind(Include = "Id,Balance,NoCuenta,PIN,IdUser,Deposito,Retiro,Cedula,typeAccount,cien,doscientos,quinientos,mil,dosmil")] CuentaBancoViewModel cuentaBanco, FormCollection collection)
        {
            USUARIO cuentaBancoTemp = db.USUARIOs.Find(cuentaBanco.Id);
            if (cuentaBanco.PIN == cuentaBancoTemp.USER_PIN_CD.ToString())
            {
                if (ModelState.IsValid)
                {

                    if (collection["options"] == "100")
                        cuentaBanco.Retiro = 100;
                    else if (collection["options"] == "200")
                        cuentaBanco.Retiro = 200;
                    else if (collection["options"] == "500")
                        cuentaBanco.Retiro = 500;
                    else if (collection["options"] == "1000")
                        cuentaBanco.Retiro = 1000;
                    else if (collection["options"] == "2000")
                        cuentaBanco.Retiro = 2000;
                    else if (cuentaBanco.Retiro > 0)
                        cuentaBanco.Retiro = cuentaBanco.Retiro;

                    cuentaBancoTemp.BALANCE_USER_CURRENT_AMT = cuentaBancoTemp.BALANCE_USER_CURRENT_AMT - cuentaBanco.Retiro;
                    cuentaBancoTemp.Card_Number_ID = cuentaBanco.Id;
                    cuentaBancoTemp.CEDULA_CD = cuentaBanco.Cedula;
                    cuentaBancoTemp.ACCOUNT_USER_CD = Convert.ToInt32(cuentaBanco.NoCuenta);
                    cuentaBancoTemp.USER_PIN_CD = Convert.ToInt32(cuentaBanco.PIN);
                    cuentaBancoTemp.ACCOUNT_TYPE_DESC = cuentaBanco.typeAccount;
                    if(cuentaBancoTemp.BALANCE_USER_CURRENT_AMT <= 0)
                    {
                        TempData["EXCEED"] = "true";
                        return RedirectToAction("Index");
                    }
                    

                    var maquinas = db.MAQUINAs.ToList();
                    var random = new Random();
                    var MaquinaATM = maquinas[random.Next(maquinas.Count)];
                    if (MaquinaATM != null) {
                        if ((MaquinaATM.BALANCE_AMT - cuentaBanco.Retiro) <= 0)
                        {
                            TempData["LACK"] = "true";
                            return RedirectToAction("Index");
                        }

                        //Resta balance al ATM
                        MaquinaATM.BALANCE_AMT = MaquinaATM.BALANCE_AMT - cuentaBanco.Retiro;
                        db.Entry(MaquinaATM).State = EntityState.Modified;
                    }
                    db.Entry(cuentaBancoTemp).State = EntityState.Modified;
                    
                    ATM_TRANSACTION transa = new ATM_TRANSACTION();
                    transa.Card_Number_ID = cuentaBanco.Id;
                    transa.TRANS_NO = Utils.Utilidades.RandomString(16);
                    transa.TRANSACTION_DT = DateTime.Now;
                    transa.ID_ATM_CD = 1;
                    transa.MAQUINA = MaquinaATM;
                    transa.TRANSACTION_TYPE_NM = Utils.Constantes.TIPO_TRANS_RETIRO;
                    transa.TRANSACTION_AMT = cuentaBanco.Retiro;
                    db.ATM_TRANSACTION.Add(transa);
                    db.SaveChanges();
                    TempData["EXITOSO"] = "true";
                    return RedirectToAction("ConfirmTrx","Transaction",new { id=transa.TRANS_NO});
                }
            }
            else
            {
                TempData["WRONGPIN"] = "true";
                return RedirectToAction("Index");
            }

            
            return RedirectToAction("Index");
        }

        // GET: CuentaBancoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO usuario = db.USUARIOs.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            var cuentaBanco = new CuentaBancoViewModel();
            cuentaBanco.PIN = usuario.USER_PIN_CD.ToString();
            cuentaBanco.NoCuenta = usuario.ACCOUNT_USER_CD.ToString();
            cuentaBanco.Cedula = usuario.CEDULA_CD;
            cuentaBanco.Id = usuario.Card_Number_ID;
            cuentaBanco.typeAccount = usuario.ACCOUNT_TYPE_DESC;
            cuentaBanco.Balance = usuario.BALANCE_USER_CURRENT_AMT;
            return View(cuentaBanco);
        }

        // POST: CuentaBancoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            USUARIO cuentaBanco = db.USUARIOs.Find(id);
            db.USUARIOs.Remove(cuentaBanco);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO uSUARIO = db.USUARIOs.Find(id);
            if (uSUARIO == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIO);
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
