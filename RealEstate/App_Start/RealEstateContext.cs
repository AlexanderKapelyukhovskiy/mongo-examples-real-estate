using MongoDB.Driver;
using RealEstate.Rentals;

namespace RealEstate.App_Start
{
    public class RealEstateContext
    {
        public MongoDatabase Database;
        public RealEstateContext()
        {
            var client = new MongoClient(Properties.Settings.Default.RealEstateConnectionString);
            var server = client.GetServer();
            Database = server.GetDatabase(Properties.Settings.Default.RealEstateDatabaseName);
        }

        public MongoCollection<Rental> Rentals => Database.GetCollection<Rental>("rentals");
    }

    public class RealEstateContextNewApi
    {
        public IMongoDatabase Database;
        public RealEstateContextNewApi()
        {
            var client = new MongoClient(Properties.Settings.Default.RealEstateConnectionString);

            Database = client.GetDatabase(Properties.Settings.Default.RealEstateDatabaseName);
        }

        public IMongoCollection<Rental> Rentals => Database.GetCollection<Rental>("rentals");
    }
}