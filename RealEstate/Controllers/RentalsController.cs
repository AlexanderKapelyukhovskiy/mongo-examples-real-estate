﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using RealEstate.App_Start;
using RealEstate.Rentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstate.Controllers
{
    public class RentalsController : Controller
    {
        public readonly RealEstateContext Context = new RealEstateContext();

        public ActionResult Index(RentalsFilter filters)
        {
            IEnumerable<Rental> rentals = FilterRentals(filters);

            var model = new RentalsList
            {
                Rentals = rentals,
                Filters = filters
            };
            return View(model);
        }

        private IEnumerable<Rental> FilterRentals(RentalsFilter filters)
        {
            IQueryable<Rental> rentals = Context.Rentals.AsQueryable().OrderBy(r => r.Price);

            if(filters.MinimumRooms.HasValue)
            {
                rentals = rentals.Where(r => r.NumberOfRooms >= filters.MinimumRooms);
            }

            if (filters.PriceLimit.HasValue)
            {
                var query = Query<Rental>.LTE(r => r.Price, filters.PriceLimit);
                rentals = rentals.Where(r => query.Inject());
            }

            return rentals;
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
    }
}