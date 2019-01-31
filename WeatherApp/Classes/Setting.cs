using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Interfaces;

namespace WeatherApp.Classes
{
    [Serializable]
    public class Setting : ISetting
    {
        public bool UseDefaultProxy { get; set; }
        public string ProxyURL { get; set; }
        public int ProxyPort { get; set; }

        public Setting(bool usedefaultproxy, string proxyurl, int proxyport)
        {
            UseDefaultProxy = usedefaultproxy;
            ProxyURL = proxyurl;
            ProxyPort = proxyport;
        }

      
    }
}
