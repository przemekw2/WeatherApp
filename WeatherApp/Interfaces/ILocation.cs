using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Classes;

namespace WeatherApp.Interfaces
{
    interface ILocation
    {
        string Name { get; set; }
        string Id { get; set; }
        string Longitude { get; set; }
        string Latitude { get; set; }
        Dictionary<string, string> CurrentWeatherDict { get; set; }
        Dictionary<string, WConditions> FiveDayWeatherDict { get; set; }
    }
}
