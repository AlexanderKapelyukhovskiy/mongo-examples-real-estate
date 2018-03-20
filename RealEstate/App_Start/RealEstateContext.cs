using MongoDB.Driver;
using RealEstate.Rentals;
using MongoDB.Driver.GridFS;

namespace RealEstate.App_Start
{
    public class RealEstateContextNewApi
    {
        public IMongoDatabase Database;
        public GridFSBucket ImageBucket;

        public RealEstateContextNewApi()
        {
            string connectionString = Properties.Settings.Default.RealEstateConnectionString;
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.ClusterConfigurator = builder => builder.Subscribe(new Log4NetMongoEvents());
            var client = new MongoClient(settings);

            Database = client.GetDatabase(Properties.Settings.Default.RealEstateDatabaseName);
            ImageBucket = new GridFSBucket(Database);
        }

        public IMongoCollection<Rental> Rentals => Database.GetCollection<Rental>("rentals");
    }
}