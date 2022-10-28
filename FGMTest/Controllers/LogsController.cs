using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FGMTest.Models;

namespace FGMTest.Controllers
{
    public class LogsController : Controller
    {
        private DB_FGMTESTEntities1 db = new DB_FGMTESTEntities1();

        // GET: Logs
        public ActionResult Logs()
        {
            return View(db.LOGS.ToList());
        }

        public ActionResult HistoricoLogins()
        {
            return View(db.HISTORICO_LOGIN.ToList());
        }

        // GET: Logs/Details/5
      
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
