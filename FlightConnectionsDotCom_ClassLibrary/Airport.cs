using System;
using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    [DebuggerDisplay("{GetFullString()}")]
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, City, Country, Name, Link);
        }

        public string GetFullString()
        {
            return $"{Code} - {City} - {Country} - {Name}";
        }

        public bool AirportIsInEurope()
        {
            return Country.Contains("Albania") ||
                    Country.Contains("Andorra") ||
                    Country.Contains("Austria") ||
                    Country.Contains("Belarus") ||
                    Country.Contains("Belgium") ||
                    Country.Contains("Bosnia") ||
                    Country.Contains("Bulgaria") ||
                    Country.Contains("Croatia") ||
                    Country.Contains("Czech") ||
                    Country.Contains("Denmark") ||
                    Country.Contains("Estonia") ||
                    Country.Contains("Finland") ||
                    Country.Contains("France") ||
                    Country.Contains("Germany") ||
                    Country.Contains("Greece") ||
                    Country.Contains("Holy See") ||
                    Country.Contains("Hungary") ||
                    Country.Contains("Iceland") ||
                    Country.Contains("Ireland") ||
                    Country.Contains("Italy") ||
                    Country.Contains("Latvia") ||
                    Country.Contains("Liechtenstein") ||
                    Country.Contains("Lithuania") ||
                    Country.Contains("Luxembourg") ||
                    Country.Contains("Malta") ||
                    Country.Contains("Moldova") ||
                    Country.Contains("Monaco") ||
                    Country.Contains("Montenegro") ||
                    Country.Contains("Netherlands") ||
                    Country.Contains("Macedonia") ||
                    Country.Contains("Norway") ||
                    Country.Contains("Poland") ||
                    Country.Contains("Portugal") ||
                    Country.Contains("Romania") ||
                    Country.Contains("Russia") ||
                    Country.Contains("San Marino") ||
                    Country.Contains("Serbia") ||
                    Country.Contains("Slovakia") ||
                    Country.Contains("Slovenia") ||
                    Country.Contains("Spain") ||
                    Country.Contains("Sweden") ||
                    Country.Contains("Switzerland") ||
                    Country.Contains("Ukraine") ||
                    Country.Contains("United Kingdom");
        }
    }
}