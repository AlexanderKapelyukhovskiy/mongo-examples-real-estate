using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using System;

namespace Tests.Quering
{
    [TestFixture]
    public class CreatingQueryDocuments
    {
        [Test]
        public void TypedQuery()
        {
            var query = Query<Person>.NE(p => p.FirstName, "anne");
            var query2 = Builders<Person>.Filter.Ne(p => p.FirstName, "anne");

            Console.WriteLine(query);
            Console.WriteLine(query2.Render(
                BsonSerializer.SerializerRegistry.GetSerializer<Person>(),
                BsonSerializer.SerializerRegistry));
        }
    }
}
