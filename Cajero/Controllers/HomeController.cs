using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cajero.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Aplicacion cajero automatico. Proyecto de Analisis y Diseño II.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Grupo 3. Cuatrimestre Mayo-Agosto 2021";

            return View();
        }
    }
}