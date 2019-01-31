using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Interfaces
{
    interface ILocation
    {
        string Name { get; set; }
        string Id { get; set; }
        string Longitude { get; set; }
        string Latitude { get; set; }
    }
}
