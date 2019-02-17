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
using System.Net;
using System.IO;
using Newtonsoft.Json;
using WeatherApp.Classes;
using System.Collections.ObjectModel;

namespace WeatherApp.Forms
{
    /// <summary>
    /// Interaction logic for LocationSearchWindow.xaml
    /// </summary>
    public partial class LocationSearchWindow : Window
    {
        MainWindow mwInstance = (MainWindow)Application.Current.MainWindow;

        private Setting setting;

        public Setting Setting
        {
            get
            {
                if (this.setting == null)
                {
                    this.setting = new Setting(true, "", 8080, "", 0, false);
                }
                return this.setting;
            }

            set
            { this.setting = value; }
        }

        private ObservableCollection<Location> dataGridItems = new ObservableCollection<Location>();
        private ObservableCollection<Location> locationsList = new ObservableCollection<Location>();

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

        public LocationSearchWindow(Setting setting)
        {
            InitializeComponent();
            Setting = setting;
            this.DataContext = dataGridItems;
            this.dataGrid1.ItemsSource = dataGridItems;
        }

        private void SearchBTN_Click(object sender, RoutedEventArgs e)
        {
            //Location foundCity = null;
            Dictionary<string, string> outputDict = FindCity(CityTB.Text);

            if (outputDict.ContainsKey("found"))
            {
                if (outputDict["found"] == "true")
                {
                    dataGridItems.Clear();
                    dataGridItems.Add(new Location(outputDict["name"], outputDict["id"], outputDict["lon"], outputDict["lat"], null , null));
                    UpdateDataGrid();
                }                
            }

        }

        //update dataGrid
        private void UpdateDataGrid()
        {
            this.dataGrid1.ItemsSource = null;
            this.dataGrid1.ItemsSource = dataGridItems;
        }

        private Dictionary<string, string> FindCity(string city)
        {
            Dictionary<string, string> outputDict = new Dictionary<string, string>();
            string URL = @"http://api.openweathermap.org/data/2.5/weather?q=" + city.Trim() + @"&APPID=" + Setting.APPID + "&units=metric";
            string json = String.Empty;

            //HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
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
                            outputDict.Add("name", rootObject.name.ToString());
                            outputDict.Add("id", rootObject.id.ToString());
                            outputDict.Add("lat", rootObject.coord.lat.ToString());
                            outputDict.Add("lon", rootObject.coord.lon.ToString());
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

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            Location selectedItem = (Location)dataGrid1.SelectedItem;
            this.locationsList.Add(selectedItem);
            mwInstance.LocationsList = this.LocationsList;
            MessageBoxButton buttons = MessageBoxButton.OK;
            MessageBoxResult result = MessageBox.Show("Location : " + selectedItem.Name + " has been added.", "New Location", buttons);
            this.Close();
        }
    }
}
