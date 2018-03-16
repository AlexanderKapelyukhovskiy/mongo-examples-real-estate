using MongoDB.Bson;
using RealEstate.App_Start;
using System.Web.Mvc;

namespace RealEstate.Controllers
{
    public class HomeController : Controller
    {
        private static RealEstateContextNewApi Context = new RealEstateContextNewApi();

        public ActionResult Index()
        {
            var buildInfoCommand = new BsonDocument("buildInfo", 1);
            var buildInfo = Context.Database.RunCommand<BsonDocument>(buildInfoCommand);
            return Content(buildInfo.ToJson(), "application/json");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}