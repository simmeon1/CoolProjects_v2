﻿using System;
using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    [DebuggerDisplay("{Code}, {City}, {Country}, {Name}")]
    public class Airport
    {
        public string Code { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public Airport(string code, string city, string country, string name)
        {
            Code = code;
            City = city;
            Country = country;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Airport airport &&
                   Code == airport.Code &&
                   City == airport.City &&
                   Country == airport.Country &&
                   Name == airport.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, City, Country, Name);
        }
    }
}