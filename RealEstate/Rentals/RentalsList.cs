using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstate.Rentals
{
    public class RentalsList
    {
        public IEnumerable<RentalViewModel> Rentals { get; set; }
        public RentalsFilter Filters { get; set; }
    }

    public class RentalViewModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string Adjustments { get; set; }
    }
}