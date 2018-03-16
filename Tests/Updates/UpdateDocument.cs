using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using System;

namespace Tests.Updates
{
    [TestFixture]
    public class UpdateDocument
    {
        [Test]
        public void TypedUpdate()
        {
            var update = Update<Person>.Set(p => p.Age, 54);
            var update2 = Builders<Person>.Update.Set(p => p.Age, 54);

            Console.WriteLine(update);
            Console.WriteLine(update2.Render(
                BsonSerializer.SerializerRegistry.GetSerializer<Person>(),
                BsonSerializer.SerializerRegistry));

        }
    }
}
