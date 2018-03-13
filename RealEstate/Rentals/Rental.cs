using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Rentals
{
    public class Rental
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Description { get; set; }
        public int NumberOfRooms { get; set; }
        public List<string> Address = new List<string>();

        [BsonRepresentation(BsonType.Double)]
        public decimal Price { get; set; }
        public string ImageId { get; internal set; }

        public List<PriceAdjustment> Adjustments = new List<PriceAdjustment>();

        public Rental()
        {

        }

        public Rental(PostRental postalRental)
        {
            Description = postalRental.Description;
            NumberOfRooms = postalRental.NumberOfRooms;
            Price = postalRental.Price;
            Address = (postalRental.Address ?? string.Empty).Split('\n').ToList();
        }

        internal void AdjustPrice(AdjustPrice adjustPrice)
        {
            var adjastment = new PriceAdjustment(adjustPrice, Price);
            Adjustments.Add(adjastment);
            Price = adjustPrice.NewPrice;
        }

        public bool HasImage()
        {
            return string.IsNullOrWhiteSpace(ImageId) == false;
        }
    }
}