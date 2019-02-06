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
            //if (this.LocationsLB.SelectedItem != null)
            //{
            //    Dictionary<string, string> CurrentWeatherdict = GetCurrentWeatherData((this.LocationsLB.SelectedItem as Location).Id); 
            //    Dictionary<string, WConditions> FiveDayWeatherDict = GetFiveDayWeather((this.LocationsLB.SelectedItem as Location).Id);
            //}
        }

        private void UpdateWeatherData()
        {
            foreach(Location location in locationsList)
            {
                location.CurrentWeatherDict = GetCurrentWeatherData(location.Id);
                location.FiveDayWeatherDict = GetFiveDayWeather(location.Id);
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

                            string temp = String.Empty;
                            string temp_min = String.Empty;
                            string temp_max = String.Empty;
                            string pressure = String.Empty;
                            string humidity = String.Empty;
                            string main = String.Empty;
                            string description = String.Empty;
                            string speed = String.Empty;
                            string deg = String.Empty;

                            outputDict.Add("found", "true");

                            #region Main
                            try { temp = rootObject.main.temp.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("temp", temp);
                            try { pressure = rootObject.main.pressure.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("pressure", pressure);
                            try { humidity = rootObject.main.humidity.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("humidity", humidity);
                            try { temp_min = rootObject.main.temp_min.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("temp_min", temp_min);
                            try { temp_max = rootObject.main.temp_max.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("temp_max", temp_max);
                            #endregion

                            #region Wind
                            try { speed = rootObject.wind.speed.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("speed", speed);
                            try { deg = rootObject.wind.deg.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("deg", deg);
                            #endregion

                            #region Weather
                            try { main = rootObject.weather[0].main.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("main", main);
                            try { description = rootObject.weather[0].description.ToString(); } catch (NullReferenceException) { }
                            outputDict.Add("description", description);
                            #endregion

                            return outputDict;
                        }

                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }

        }

        private Dictionary<string, WConditions> GetFiveDayWeather(string ID)
        {

            Dictionary<string, WConditions> outputDict = new Dictionary<string, WConditions>();
            string URL = @"http://api.openweathermap.org/data/2.5/forecast?id=" + ID + @"&APPID=53849b8462e783dd24f9bdfb43563129&units=metric";
            //string URL = @"http://api.openweathermap.org/data/2.5/weather?id=" + ID + @"&APPID=53849b8462e783dd24f9bdfb43563129&units=metric";
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
                            //WeatherApp.JSON_Classes_Five_Day.List list5DayForecast = JsonConvert.DeserializeObject<WeatherApp.JSON_Classes_Five_Day.List>(responseText);
                            WeatherApp.JSON_Classes_Five_Day.RootObject rootObject = JsonConvert.DeserializeObject<WeatherApp.JSON_Classes_Five_Day.RootObject>(responseText);
                            for (int i = 0; i < rootObject.list.Count; i++)
                            {
                                string dt = String.Empty;
                                string dt_txt = String.Empty;
                                string temp = String.Empty;
                                string temp_min = String.Empty;
                                string temp_max = String.Empty;
                                string pressure = String.Empty;
                                string humidity = String.Empty;
                                string main = String.Empty;
                                string description = String.Empty;
                                string all = String.Empty;
                                string speed = String.Empty;
                                string deg = String.Empty;
                                string rain = String.Empty;
                                string snow = String.Empty;

                                try { dt = rootObject.list[i].dt.ToString(); } catch(NullReferenceException) { }
                                try { dt_txt = rootObject.list[i].dt_txt.ToString(); } catch (NullReferenceException) { }
                                try { temp = rootObject.list[i].main.temp.ToString(); } catch (NullReferenceException) { }
                                try { temp_min = rootObject.list[i].main.temp_min.ToString(); } catch (NullReferenceException) { }
                                try { temp_max = rootObject.list[i].main.temp_max.ToString(); } catch (NullReferenceException) { }
                                try { pressure = rootObject.list[i].main.pressure.ToString(); } catch (NullReferenceException) { }
                                try { humidity = rootObject.list[i].main.humidity.ToString(); } catch (NullReferenceException) { }
                                try { main = rootObject.list[i].weather[0].main.ToString(); } catch (NullReferenceException) { }
                                try { description = rootObject.list[i].weather[0].description.ToString(); } catch (NullReferenceException) { }
                                try { all = rootObject.list[i].clouds.all.ToString(); } catch (NullReferenceException) { }
                                try { speed = rootObject.list[i].wind.speed.ToString(); } catch (NullReferenceException) { }
                                try { deg = rootObject.list[i].wind.deg.ToString(); } catch (NullReferenceException) { }
                                try { rain = rootObject.list[i].rain.__invalid_name__3h.ToString(); } catch (NullReferenceException) { }
                                try { snow = rootObject.list[i].snow.__invalid_name__3h.ToString(); } catch (NullReferenceException) { }

                                outputDict.Add(rootObject.list[i].dt_txt, new WConditions(
                                    ID, dt, dt_txt, temp, temp_min, temp_max, pressure, humidity, main, description, all, speed, deg, rain, snow));
                            }

                            return outputDict;

                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }

        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateWeatherData();
        }
    }
}
