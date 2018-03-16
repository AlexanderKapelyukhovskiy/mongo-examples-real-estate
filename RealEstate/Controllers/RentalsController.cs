using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using RealEstate.App_Start;
using RealEstate.Rentals;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RealEstate.Controllers
{
    public class RentalsController : Controller
    {
        public readonly RealEstateContext Context = new RealEstateContext();
        private readonly RealEstateContextNewApi ContextNew = new RealEstateContextNewApi();

        public async Task<ActionResult> Index(RentalsFilter filters)
        {
            FilterDefinition<Rental> filterDefinition = filters.ToFilterDefinitio();

            var rentals = await ContextNew.Rentals
                .Find(filterDefinition)
                //.Sort(Builders<Rental>.Sort.Ascending(r => r.Price))
                .SortBy(r => r.Price)
                .ToListAsync();

            var model = new RentalsList
            {
                Rentals = rentals,
                Filters = filters
            };
            return View(model);
        }

        public ActionResult Post()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Post(PostRental postalRental)
        {
            var rental = new Rental(postalRental);
            Context.Rentals.Insert(rental);

            return RedirectToAction("Index");
        }

        public ActionResult AdjustPrice(string id)
        {
            Rental rental = GetRental(id);
            return View(rental);
        }

        private Rental GetRental(string id)
        {
            return Context.Rentals.FindOneById(new ObjectId(id));
        }

        [HttpPost]
        public ActionResult AdjustPrice(string id, AdjustPrice adjustPrice)
        {
            var rental = GetRental(id);
            rental.AdjustPrice(adjustPrice);
            Context.Rentals.Save(rental);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            Context.Rentals.Remove(Query.EQ("_id", new ObjectId(id)));
            return RedirectToAction("Index");
        }

        public ActionResult AttachImage(string id)
        {
            var rental = GetRental(id);
            return View(rental);
        }

        [HttpPost]
        public ActionResult AttachImage(string id, HttpPostedFileBase file)
        {
            var rental = GetRental(id);
            if (rental.HasImage())
            {
                DeleteImage(rental);
            }
            StoreImage(file, rental);

            return RedirectToAction("Index");
        }

        private void DeleteImage(Rental rental)
        {
            Context.Database.GridFS.DeleteById(rental.ImageId);
            rental.ImageId = null;
            Context.Rentals.Save(rental);
        }

        private void StoreImage(HttpPostedFileBase file, Rental rental)
        {
            rental.ImageId = ObjectId.GenerateNewId().ToString();
            Context.Rentals.Save(rental);

            var options = new MongoGridFSCreateOptions
            {
                Id = rental.ImageId,
                ContentType = file.ContentType
            };
            Context.Database.GridFS.Upload(file.InputStream, file.FileName, options);
        }

        public ActionResult GetImage(string id)
        {
            var image = Context.Database.GridFS.FindOneById(id);
            if(image == null)
            {
                return HttpNotFound();
            }
            return File(image.OpenRead(), image.ContentType);
        }

        public string PriceDistribution()
        {
            return new QueryPriceDistribution().Run(Context.Rentals).ToJson();
        }
    }
}