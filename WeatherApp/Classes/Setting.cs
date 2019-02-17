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
        public string APPID { get; set; }
        public int UpdateInterval { get; set; }
        public bool TrayNotification { get; set; }

        public Setting(bool usedefaultproxy, string proxyurl, int proxyport, string appid, int updateinterval, bool traynotifications)
        {
            this.UseDefaultProxy = usedefaultproxy;
            this.ProxyURL = proxyurl;
            this.ProxyPort = proxyport;
            this.APPID = appid;
            this.UpdateInterval = updateinterval;
            this.TrayNotification = traynotifications;
        }

      
    }
}
