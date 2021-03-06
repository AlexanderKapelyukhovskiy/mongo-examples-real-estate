﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstate.Rentals
{
    public class PostRental
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
    }
}