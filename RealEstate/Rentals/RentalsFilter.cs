using MongoDB.Driver;

namespace RealEstate.Rentals
{
    public class RentalsFilter
    {
        public decimal? PriceLimit { get; set; }
        public int? MinimumRooms { get; set; }

        public FilterDefinition<Rental> ToFilterDefinitio()
        {
            var filterDefinition = Builders<Rental>.Filter.Empty;

            if (MinimumRooms.HasValue)
            {
                filterDefinition = Builders<Rental>.Filter
                    .Where(r => r.NumberOfRooms >= (MinimumRooms ?? 0));
            }

            if (PriceLimit.HasValue)
            {
                filterDefinition &= Builders<Rental>.Filter.Lte(r => r.Price, PriceLimit);
            }

            return filterDefinition;
        }
    }

    
}