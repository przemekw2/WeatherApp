using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherApp.JSON_Classes_Five_Day
{
    public class Snow
    {
        private double _3h_;

        [JsonProperty("3h")]
        public double __invalid_name__3h
        {
            get
            {
                if (this._3h_ == null)
                {
                    this._3h_ = 0;
                }
                return this._3h_;
            }
            set { this._3h_ = value; }
        }
    }
}
