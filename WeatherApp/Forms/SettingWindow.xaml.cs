using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeatherApp.Classes;

namespace WeatherApp.Forms
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        MainWindow mwInstance = (MainWindow)Application.Current.MainWindow;
        private Setting setting;

        public Setting Setting
        {
            get
            {
                if (this.setting == null)
                {
                    this.setting = new Setting(true, "", 8080, "", 0);
                }
                return this.setting;
            }

            set
            { this.setting = value; }
        }

        public SettingWindow(Setting setting)
        {
            InitializeComponent();
            Setting = setting;
            //read proxy setting
            if (Setting.UseDefaultProxy)
            {
                ProxyDefaultRB.IsChecked = true;
            }
            else
            {
                ProxyCustomRB.IsChecked = true;
                ProxyUrlTB.Text = Setting.ProxyURL;
                ProxyNUD.Value = Setting.ProxyPort;
            }

            //read miscellaneous setting
            APIKEYTB.Text = Setting.APPID;
            IntervalNUD.Value = Setting.UpdateInterval;
        }

        private void OkBTN_Click(object sender, RoutedEventArgs e)
        {
            //save proxy setting
            if (ProxyDefaultRB.IsChecked == true)
            {
                Setting.UseDefaultProxy = true;
            }

            if (ProxyCustomRB.IsChecked == true)
            {
                Setting.UseDefaultProxy = false;
                Setting.ProxyURL = ProxyUrlTB.Text;
                Setting.ProxyPort = (int)ProxyNUD.Value;
            }

            //save miscellaneous setting
            Setting.APPID = APIKEYTB.Text;
            Setting.UpdateInterval = (int)IntervalNUD.Value;

            mwInstance.Setting = this.Setting;
            this.Close();
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
