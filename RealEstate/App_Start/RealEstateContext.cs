using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using RealEstate.Rentals;
using System.Diagnostics;

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
            string connectionString = Properties.Settings.Default.RealEstateConnectionString;
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.ClusterConfigurator = builder => builder.Subscribe(new Log4NetMongoEvents());
            var client = new MongoClient(settings);

            Database = client.GetDatabase(Properties.Settings.Default.RealEstateDatabaseName);
        }

        public IMongoCollection<Rental> Rentals => Database.GetCollection<Rental>("rentals");
    }
}