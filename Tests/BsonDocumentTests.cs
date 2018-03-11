using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class BsonDocumentTests
    {
        public BsonDocumentTests()
        {
            JsonWriterSettings.Defaults.Indent = true;
        }
        [Test]
        public void EmptyDocument() {
            var document = new BsonDocument();
            Console.WriteLine(document);
        }

        [Test]
        public void AddElements()
        {
            var person = new BsonDocument
            {
                { "age", new BsonInt32(42) },
                { "isAlive", true },
                { "fistName", new BsonString("Bob") }
            };
            Console.WriteLine(person);
        }

        [Test]
        public void AddingArrays()
        {
            var person = new BsonDocument();
            person.Add("address", new BsonArray(new[] { "101 Some Road", " Unit 501" }));
            Console.WriteLine(person);
        }

        [Test]
        public void EmbeddedDocument()
        {
            var person = new BsonDocument {
                { "contact", new BsonDocument {
                    { "phone", "555-55-55" },
                    { "email", "test@test.com" } }
                }
            };
            Console.WriteLine(person);
        }

        [Test]
        public void BsonValueConversions()
        {
            var person = new BsonDocument {
                { "age", 54 },
                { "contact", new BsonDocument {
                    { "phone", "555-55-55" },
                    { "email", "test@test.com" } }
                }
            };
            Console.WriteLine(person["age"].AsInt32 + 10);
            Console.WriteLine(person["age"].ToDouble() + 10);
            Console.WriteLine(person["age"].IsInt32);
            Console.WriteLine(person["age"].IsString);
        }

        [Test]
        public void ToBson()
        {
            var person = new BsonDocument {
                { "firstName", "bob" }
            };
            var bson = person.ToBson();
            Console.WriteLine(BitConverter.ToString(bson));
            var deserializedPerson = BsonSerializer.Deserialize<BsonDocument>(bson);
            Console.WriteLine(deserializedPerson);
        }
    }
}
