using MongoDB.Bson;
using NUnit.Framework;
using RealEstate.Rentals;

namespace Tests.Rentals
{
    [TestFixture]
    class RentalTests// : AssertionHelper
    {
        [Test]
        public void ToDocument_RentalWithPrice_PriceRepresentedAsDouble()
        {
            var rental = new Rental() { Price = 100.5m };
            var document = rental.ToBsonDocument();
            Assert.That(document["Price"].BsonType, Is.EqualTo(BsonType.Double));
        }

        [Test]
        public void ToDocument_RentalWithId_IdIsRepresentedAsObjectId()
        {
            var rental = new Rental();
            rental.Id = ObjectId.GenerateNewId().ToString();

            var document = rental.ToBsonDocument();
            Assert.That(document["_id"].BsonType, Is.EqualTo(BsonType.ObjectId));

        }
    }
}
