using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    [BsonIgnoreExtraElements]
    public class Person
    {
        [BsonId]
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public List<string> Address = new List<string>();
        [BsonIgnoreIfNull]
        public Contact Contact;
        [BsonIgnore]
        public string IgnoreMe { get; set; }
        [BsonElement("New")]
        public string Old { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal NetWorth { get; set; }

        public DateTime BirthDay { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local, DateOnly = true)]
        public DateTime BirthDayLocal { get; set; }
    }

    public class Contact
    {
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    [TestFixture]
    public class PocoTests
    {
        public PocoTests()
        {
            JsonWriterSettings.Defaults.Indent = true;
        }

        [Test]
        public void SerializationAttributes()
        {
            var person = new Person
            {
                FirstName = "bob",
                IgnoreMe = "some value",
                NetWorth = 100.5m,
                BirthDay = DateTime.Now,
                BirthDayLocal = new DateTime(2014, 1, 1)
            };

            Console.WriteLine(person.ToJson());
        }


        [Test]
        public void Automatic()
        {
            var person = new Person { FirstName = "bob", Age = 54 };
            person.Address.Add("101 Some Road");
            person.Address.Add("Unit 501");
            person.Contact = new Contact { Email = "test@test.com", Phone = "555-55-55" };

            Console.WriteLine(person.ToJson());
        }

        [Test]
        public void TimeinBson()
        {
            var doc = new BsonDocument
            {
                {
                    "BirthDate", new DateTime(2014, 1, 2, 11, 30, 0)
                }
            };
            Console.WriteLine(doc);
        }
    }
}
