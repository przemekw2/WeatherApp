using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Interfaces;

namespace WeatherApp.Classes
{
    [Serializable]
    public class WConditions : IWConditions
    {
        public string TimeStamp { get; set; }
        public string TimeStampTxt { get; set; }
        public string Temp { get; set; }
        public string TempMin { get; set; }
        public string TempMax { get; set; }
        public string Pressure { get; set; }
        public string Humidity { get; set; }
        public string Conditions { get; set; }
        public string ConditionsDesc { get; set; }
        public string Cloud { get; set; }
        public string WindSpeed { get; set; }
        public string WindDeg { get; set; }
        public string Rain { get; set; }
        public string Snow { get; set; }

        public WConditions(string timestamp, string timestamptxt, string temp,
            string tempmin, string tempmax, string pressure, string humidity, string conditions,
            string conditionsdesc, string cloud, string windspeed, string winddeg, string rain,
            string snow)
        {
            this.TimeStamp = timestamp;
            this.TimeStampTxt = timestamptxt;
            this.Temp = temp;
            this.TempMin = tempmin;
            this.TempMax = tempmax;
            this.Pressure = pressure;
            this.Humidity = humidity;
            this.Conditions = conditions;
            this.ConditionsDesc = conditionsdesc;
            this.Cloud = cloud;
            this.WindSpeed = windspeed;
            this.WindDeg = winddeg;
            this.Rain = rain;
            this.Snow = snow;
        }


    }
}
