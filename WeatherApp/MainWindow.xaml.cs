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
using System.Windows.Navigation;
using System.Windows.Shapes;
//-------------------------------

using System.Text.RegularExpressions;
//using System.ServiceModel.Syndication;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using WeatherApp.Forms;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using WeatherApp.Classes;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string applicationDirPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            + System.IO.Path.DirectorySeparatorChar;
        private const string settingFileName = "Setting.dat";
        private const string dataFileName = "Data.dat";

        private Setting setting;

        public Setting Setting
        {
            get
            {
                if (this.setting == null)
                {
                    this.setting = new Setting(true, "", 8080);
                }
                return this.setting;
            }

            set
            { this.setting = value; }
        }

        private ObservableCollection<Location> locationsList;

        public ObservableCollection<Location> LocationsList
        {
            get
            {
                if (this.locationsList == null)
                {
                    this.locationsList = new ObservableCollection<Location>();
                }
                return this.locationsList;
            }
            set { this.locationsList = value; }
        }


        public MainWindow()
        {
            InitializeComponent();
            //read setting file
            this.Setting = (Setting)DeserializeObject(applicationDirPath + settingFileName);
        }

        private void SearchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocationSearchWindow locationSearchWindow = new LocationSearchWindow();
            locationSearchWindow.ShowDialog();
        }

        private void SettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow(Setting);
            //settingWindow.Setting = Setting;
            settingWindow.ShowDialog();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void SerializeObject(object obj, string filePath)
        {
            
            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream output = File.Create(filePath))
            {
                formatter.Serialize(output, obj);
            }
        }

        private object DeserializeObject(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (Stream input = File.OpenRead(filePath))
                    {
                        return formatter.Deserialize(input);
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private void Weather_App_Closed(object sender, EventArgs e)
        {
            SerializeObject(Setting, applicationDirPath + settingFileName);
        }
    }
}
