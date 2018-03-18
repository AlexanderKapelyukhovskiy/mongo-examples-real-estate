using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using RealEstate.App_Start;
using RealEstate.Rentals;

using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.Linq;

namespace RealEstate.Controllers
{
    public class RentalsController : Controller
    {
        public readonly RealEstateContext Context = new RealEstateContext();
        public readonly RealEstateContextNewApi ContextNew = new RealEstateContextNewApi();

        public async Task<ActionResult> Index(RentalsFilter filters)
        {
            var rentals = await FilterRental(filters)
                .Select(r => new RentalViewModel
                {
                    Id = r.Id,
                    Address = r.Address,
                    Description = r.Description,
                    NumberOfRooms = r.NumberOfRooms,
                    Price = r.Price,
                    //Adjustments = r.Adjustments.Select(a => a.Describe()).LastOrDefault()
                })
                .OrderBy(r => r.Price)
                .ThenByDescending(r => r.NumberOfRooms)
                .ToListAsync();

            var model = new RentalsList
            {
                Rentals = rentals,
                Filters = filters
            };
            return View(model);
        }

        private IMongoQueryable<Rental> FilterRental(RentalsFilter filter)
        {
            IMongoQueryable<Rental> rentals = ContextNew.Rentals.AsQueryable();

            if (filter.MinimumRooms.HasValue)
            {
                rentals = rentals.Where(r => r.NumberOfRooms >= filter.MinimumRooms);
            }

            if (filter.PriceLimit.HasValue)
            {
                rentals = rentals.Where(r => r.Price <= filter.PriceLimit);
            }

            return rentals;
        }

        public ActionResult Post()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Post(PostRental postalRental)
        {
            var rental = new Rental(postalRental);
            await ContextNew.Rentals.InsertOneAsync(rental);

            return RedirectToAction("Index");
        }

        public ActionResult AdjustPrice(string id)
        {
            Rental rental = GetRental(id);
            return View(rental);
        }

        private Rental GetRental(string id)
        {
            return ContextNew.Rentals
                //.Find(Builders<Rental>.Filter.Where(r => r.Id == id)).FirstOrDefault();
                .Find(r => r.Id == id)
                .FirstOrDefault();
        }

        [HttpPost]
        public async Task<ActionResult> AdjustPrice(string id, AdjustPrice adjustPrice)
        {
            var rental = GetRental(id);
            rental.AdjustPrice(adjustPrice);
            var options = new UpdateOptions
            {
                IsUpsert = true
            };
            await ContextNew.Rentals.ReplaceOneAsync(r => r.Id == id, rental, options);
            //Context.Rentals.Save(rental);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> AdjustPriceWithModifications(string id, AdjustPrice adjustPrice)
        //public async Task<ActionResult> AdjustPrice(string id, AdjustPrice adjustPrice)
        {
            var rental = GetRental(id);
            var adjustment = new PriceAdjustment(adjustPrice, rental.Price);
            var modification = Builders<Rental>.Update
                .Push(r => r.Adjustments, adjustment)
                .Set(r => r.Price, adjustPrice.NewPrice);
            await ContextNew.Rentals.UpdateOneAsync(r => r.Id == id, modification);

            //Context.Rentals.Save(rental);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string id)
        {
            await ContextNew.Rentals.DeleteOneAsync(r => r.Id == id);
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
            return new QueryPriceDistribution().RunLinq(ContextNew.Rentals).ToJson();
        }
    }
}