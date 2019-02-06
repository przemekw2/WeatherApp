using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Interfaces
{
    interface IWConditions
    {
        string ID { get; set; }
        string TimeStamp { get; set; }
        string TimeStampTxt { get; set; }
        string Temp { get; set; }
        string TempMin { get; set; }
        string TempMax { get; set; }
        string Pressure { get; set; }
        string Humidity { get; set; }
        string Conditions { get; set; }
        string ConditionsDesc { get; set; }
        string Cloud { get; set; }
        string WindSpeed { get; set; }
        string WindDeg { get; set; }
        string Rain { get; set; }

    }
}
