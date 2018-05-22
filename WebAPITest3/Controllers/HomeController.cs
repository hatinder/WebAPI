using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPI.data1;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;

namespace WebAPITest3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //TODO: Add combined view to various features
            var repo = new StockRepository(new StockContext());
            //var results = repo.GetAllSecurities();
            //var results = repo.GetAllSecuritiesWithPrices().Take(10).ToList();
            var results = repo.GetSpecificSecurity("AAL");
            //var results = repo.GetSpecificSecurityBetweenDates("AAL", DateTime.Parse("03/01/2018 00:00:00"), DateTime.Parse("25/01/2018"));
            return View(results);
        }
    }
}
