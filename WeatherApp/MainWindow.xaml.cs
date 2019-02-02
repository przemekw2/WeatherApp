﻿using System;
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
using System.Net;

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
            //read data file
            this.LocationsList = (ObservableCollection<Location>)DeserializeObject(applicationDirPath + dataFileName);
            //bind ListBox itemsource
            LocationsLB.ItemsSource = LocationsList;
        }

        private void UpdateListBox()
        {
            this.LocationsLB.ItemsSource = null;
            this.LocationsLB.ItemsSource = LocationsList;
        }

        private void SearchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocationSearchWindow locationSearchWindow = new LocationSearchWindow(Setting);
            locationSearchWindow.LocationsList = LocationsList;
            locationSearchWindow.ShowDialog();
            this.UpdateListBox();
        }

        private void SettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow(Setting);
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
            SerializeObject(LocationsList, applicationDirPath + dataFileName);
        }

        private void LocationsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.LocationsLB.SelectedItem != null)
            {
                Dictionary<string, string> CurrentWeather = GetCurrentWeatherData((this.LocationsLB.SelectedItem as Location).Id); 
            }
        }

        private Dictionary<string, string> GetCurrentWeatherData(string ID)
        {

            Dictionary<string, string> outputDict = new Dictionary<string, string>();
            string URL = @"http://api.openweathermap.org/data/2.5/weather?id=" + ID + @"&APPID=53849b8462e783dd24f9bdfb43563129&units=metric";
            string json = String.Empty;

            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            request.UserAgent = "Googlebot/1.0 (googlebot@googlebot.com http://googlebot.com/)";

            if (!Setting.UseDefaultProxy)
            {
                WebProxy proxy = new WebProxy(Setting.ProxyURL, Setting.ProxyPort);
                request.Proxy = proxy;
            }
            else
            {
                IWebProxy proxy = WebRequest.GetSystemWebProxy();
                request.Proxy = proxy;
            }

            if (request.Proxy != null)
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        var encoding = ASCIIEncoding.ASCII;
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string responseText = reader.ReadToEnd();
                            WeatherApp.JSON_Classes.RootObject rootObject = JsonConvert.DeserializeObject<WeatherApp.JSON_Classes.RootObject>(responseText);
                            outputDict.Add("found", "true");

                            #region Main
                            outputDict.Add("temp", rootObject.main.temp.ToString());
                            outputDict.Add("pressure", rootObject.main.temp.ToString());
                            outputDict.Add("humidity", rootObject.main.humidity.ToString());
                            outputDict.Add("temp_min", rootObject.main.temp_min.ToString());
                            outputDict.Add("temp_max", rootObject.main.temp_max.ToString());
                            #endregion

                            #region Wind
                            outputDict.Add("speed", rootObject.wind.speed.ToString());
                            outputDict.Add("deg", rootObject.wind.deg.ToString());
                            #endregion

                            #region Weather
                            foreach (var item in rootObject.weather)
                            {
                                outputDict.Add("main", item.main.ToString());
                                outputDict.Add("description", item.description.ToString());
                            }
                            #endregion
                            
                            return outputDict;
                        }

                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                outputDict.Add("found", "false");
                outputDict.Add("error description", ex.Message);
                return outputDict;
            }

        }
    }
}
