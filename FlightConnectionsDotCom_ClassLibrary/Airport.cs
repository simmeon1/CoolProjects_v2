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
        public string Link { get; set; }
        public Airport(string code, string city, string country, string name, string link)
        {
            Code = code;
            City = city;
            Country = country;
            Name = name;
            Link = link;
        }

        public override bool Equals(object obj)
        {
            return obj is Airport airport &&
                   Code == airport.Code &&
                   City == airport.City &&
                   Country == airport.Country &&
                   Name == airport.Name &&
                   Link == airport.Link;
        }

        public string GetFullString()
        {
            return $"{Code} - {City} - {Country} - {Name}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, City, Country, Name, Link);
        }
    }
}