using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Interfaces
{
    interface ISetting
    {
        bool UseDefaultProxy { get; set; }
        string ProxyURL { get; set; }
        int ProxyPort { get; set; }
        string APPID { get; set; }
        int UpdateInterval { get; set; }
        bool TrayNotification { get; set; }
    }
}
