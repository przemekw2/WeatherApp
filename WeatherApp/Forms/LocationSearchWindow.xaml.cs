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

        public LocationSearchWindow()
        {
            InitializeComponent();
        }

        private void SearchBTN_Click(object sender, RoutedEventArgs e)
        {
            Location foundCity = null;
            Dictionary<string, string> outputDict = FindCity(CityTB.Text);

            if (outputDict.ContainsKey("found"))
            {
                if (outputDict["found"] == "true")
                {
                    foundCity = new Location(outputDict["name"], outputDict["id"], outputDict["lon"], outputDict["lat"]);
                    //this.locationsList.Add(foundCity);
                    //mwInstance.LocationsList = this.LocationsList;
                }

                
            }

        }

        private Dictionary<string, string> FindCity(string city)
        {
            Dictionary<string, string> outputDict = new Dictionary<string, string>();
            string URL = @"http://api.openweathermap.org/data/2.5/weather?q=" + city.Trim() + @"&APPID=53849b8462e783dd24f9bdfb43563129&units=metric";
            string json = String.Empty;

            WebRequest request = WebRequest.Create(URL);
            request.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                using (WebResponse response = request.GetResponse())
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
    }
}
