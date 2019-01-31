using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Interfaces;

namespace WeatherApp.Classes
{
    public class Location : ILocation
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public Location(string name, string id, string longitude, string latitude)
        {
            this.Name = name;
            this.Id = id;
            this.Longitude = longitude;
            this.Latitude = latitude;
        }


    }
}
