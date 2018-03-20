using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using RealEstate.App_Start;
using RealEstate.Rentals;

using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson.IO;
using MongoDB.Driver.Linq;

namespace RealEstate.Controllers
{
    public class RentalsController : Controller
    {
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
        public async Task<ActionResult> AttachImage(string id, HttpPostedFileBase file)
        {
            var rental = GetRental(id);
            if (rental.HasImage())
            {
                await DeleteImageAsync(rental);
            }
            await StoreImageAsync(file, rental);

            return RedirectToAction("Index");
        }

        private async Task DeleteImageAsync(Rental rental)
        {
            await ContextNew.ImageBucket.DeleteAsync(new ObjectId(rental.ImageId));
            await SetImagelIdAsync(rental.Id, null);
        }

        private async Task SetImagelIdAsync(string rentalId, string imageId)
        {
            await ContextNew.Rentals.UpdateOneAsync(r => r.Id == rentalId,
                Builders<Rental>.Update.Set(r => r.ImageId, imageId));
        }

        private async Task StoreImageAsync(HttpPostedFileBase file, Rental rental)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("contentType", file.ContentType)
            };

            var fileId = await ContextNew.ImageBucket.UploadFromStreamAsync(file.FileName, file.InputStream, options);
            rental.ImageId = fileId.ToString();

            await SetImagelIdAsync(rental.Id, rental.ImageId);
        }

        public async Task<ActionResult> GetImageAsync(string id)
        {
            try
            {
                var stream = await ContextNew.ImageBucket.OpenDownloadStreamAsync(new ObjectId(id));
                var contentType = stream.FileInfo.Metadata["contentType"].AsString;
                return File(stream, contentType);
            }
            catch (GridFSFileNotFoundException)
            {
                return HttpNotFound();
            }
        }

        public string PriceDistribution()
        {
            return new QueryPriceDistribution().RunLinq(ContextNew.Rentals).ToJson();
        }

        public ActionResult JoinPreLookup()
        {
            var rentals = ContextNew.Rentals.Find(new BsonDocument()).ToList();
            var rentalZips = rentals.Select(r => r.ZipCode).Distinct().ToArray();

            var zipsById = ContextNew.Database.GetCollection<ZipCode>("zips")
                .Find(z => rentalZips.Contains(z.Id))
                .ToList()
                .ToDictionary(d => d.Id);

            ZipCode GetZipCode(string zip)
            {
                if (zip!= null && zipsById.ContainsKey(zip))
                {
                    return zipsById[zip];
                }

                return null;
            }

            var report = rentals
                .Select(r => new {Rental = r, ZipCode = GetZipCode(r.ZipCode)})
                .ToList();

            return Content(report.ToJson(new JsonWriterSettings {OutputMode = JsonOutputMode.Strict}),
                "application/json");

        }

        public ActionResult JoinWithLookup()
        {
            var report = ContextNew.Rentals.Aggregate()
                .Lookup<Rental, ZipCode, RentalWithZipCode>(
                    ContextNew.Database.GetCollection<ZipCode>("zips"),
                    r => r.ZipCode,
                    z => z.Id,
                    d => d.ZipCodes)
                .ToList();

            return Content(report.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }),
                "application/json");

        }
    }
}