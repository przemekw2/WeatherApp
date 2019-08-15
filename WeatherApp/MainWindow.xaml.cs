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
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using WeatherApp.Forms;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using WeatherApp.Classes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Location tempLocation = null;
        private TrayIcon trayIcon;

        private HttpWebRequest request;
        Dictionary<string, string> currentWeatherDict = new Dictionary<string, string>();
        //Dictionary<string, WConditions> fiveDayWeatherDict = new Dictionary<string, WConditions>();

        private string applicationDirPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            + System.IO.Path.DirectorySeparatorChar;


        #region DispatcherTimers definition
        private DispatcherTimer dispatcherTimerUpdate = new DispatcherTimer();
        private DispatcherTimer dispatcherTimerNetwork = new DispatcherTimer();
        private DispatcherTimer dispatcherTimerTrayNotifications = new DispatcherTimer();
        private const int dispatcherTimerNetworkVal = 30;
        private const string URLStr = "www.google.pl";
        #endregion

        #region Setting definitions
        private const string settingFileName = "Setting.dat";
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
        #endregion

        #region Locations definition
        private const string dataFileName = "Data.dat";
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
        #endregion

        public MainWindow()
        {
            InitializeComponent();
              
            //read setting file
            this.Setting = (Setting)DeserializeObject(applicationDirPath + settingFileName);
            //read data file and create locations list
            this.LocationsList = (ObservableCollection<Location>)DeserializeObject(applicationDirPath + dataFileName);


            #region tray icon definition
            //tray icon
            trayIcon = new TrayIcon(this, GetLocationsList());
            Closing += OnClosingWindow;
            #endregion

            //bind ListBox itemsource
            LocationsLB.ItemsSource = LocationsList;

            if (NetworkConnectionStatus(URLStr))
            {
                StatusLB.Content = "Internet Connection OK";
                UpdateWeatherData();
            }
            else { StatusLB.Content = "No Internet Connection"; }

            //Set Dispatcher Timers
            SetDispatcherTimers();
            //ShowNotifications();
        }

        private void ShowNotifications()
        {
            foreach (Location locationItem in locationsList)
            {
                string locationName = locationItem.Name;
                string locationDetails = String.Empty;
                locationDetails += "Temperature: " + locationItem.CurrentWeatherDict["temp"] + "\n";
                locationDetails += "Pressure: " + locationItem.CurrentWeatherDict["pressure"] + "\n";
                locationDetails += "Humidity: " + locationItem.CurrentWeatherDict["humidity"] + "\n";
                locationDetails += "Conditions: " + locationItem.CurrentWeatherDict["main"] + " : " + locationItem.CurrentWeatherDict["description"] + "\n";
                locationDetails += "Wind: Speed: " + locationItem.CurrentWeatherDict["speed"] + " ,direction : " + locationItem.CurrentWeatherDict["deg"] + "\n";
                trayIcon.ShowTrayInformation(locationName, locationDetails);
                System.Threading.Thread.Sleep(10000);
            }
        }

        private List<string> GetLocationsList()
        {
            List<string> locList = new List<string>();
            try
            {
                foreach (Location locationItem in locationsList)
                {
                    locList.Add(locationItem.Name);
                }
            }
            catch (NullReferenceException) { }
            

            return locList;
        }

        private void OnClosingWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void SetDispatcherTimers()
        {
            dispatcherTimerNetwork.Tick += dispatcherTimerNetwork_Tick;
            dispatcherTimerNetwork.Interval = new TimeSpan(0, 0, dispatcherTimerNetworkVal);
            dispatcherTimerNetwork.Start();
            dispatcherTimerUpdate.Tick += dispatcherTimerUpdate_Tick;
            dispatcherTimerUpdate.Interval = new TimeSpan(0, setting.UpdateInterval, 0);
            dispatcherTimerUpdate.Start();
            dispatcherTimerTrayNotifications.Tick += dispatcherTimerTrayNotifications_Tick;
            dispatcherTimerTrayNotifications.Interval = new TimeSpan(0, setting.UpdateInterval, 0);
            SetDispatcherTimerTNotifications();
        }

        private void SetDispatcherTimerTNotifications()
        {
            if (Setting.TrayNotification)
            {
                dispatcherTimerTrayNotifications.Start();
            }
            else
            {
                dispatcherTimerTrayNotifications.Stop();
            }
        }

        private void dispatcherTimerNetwork_Tick(object sender, EventArgs e)
        {
            if (NetworkConnectionStatus(URLStr))
            {
                StatusLB.Content = "Internet Connection OK";
            }
            else
            {
                StatusLB.Content = "No Internet Connection";
            }
        }

        private void dispatcherTimerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateWeatherData();
        }

        private void dispatcherTimerTrayNotifications_Tick(object sender, EventArgs e)
        {
            ShowNotifications();
        }

        private void UpdateListBox()
        {
            this.LocationsLB.ItemsSource = null;
            this.LocationsLB.ItemsSource = LocationsList;
            //update context menu
            trayIcon.UpdateContextMenu(GetLocationsList());
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
            SetDispatcherTimerTNotifications();
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
                tempLocation = this.LocationsLB.SelectedItem as Location;
                if (tempLocation.CurrentWeatherDict == null || tempLocation.FiveDayWeatherDict == null) UpdateWeatherData();
                UpdateFormControls(tempLocation);
            }
        }

        private void UpdateWeatherData()
        {
            //update forecast data
            foreach (Location location in locationsList)
            {
                Task<Dictionary<string, string>> taskCurrentWeatherDict = GetCurrentWeatherDataAsync(location.Id, Setting.APPID);
                location.CurrentWeatherDict = taskCurrentWeatherDict.Result;
                //location.CurrentWeatherDict = GetCurrentWeatherData(location.Id, Setting.APPID);
                //location.FiveDayWeatherDict = GetFiveDayWeather(location.Id, Setting.APPID);
                Task<Dictionary<string, WConditions>> taskFiveDayWeatherDict = GetFiveDayWeatherAsync(location.Id, Setting.APPID);
                location.FiveDayWeatherDict = taskFiveDayWeatherDict.Result;
            }

            //update form controls
            if (this.LocationsLB.SelectedItem != null)
            {
                tempLocation = this.LocationsLB.SelectedItem as Location;
                UpdateFormControls(tempLocation);
            }
            else
            {
                if(locationsList.Count != 0) UpdateFormControls(locationsList.First());
            }
        }

        private void UpdateFormControls(Location tempLocation)
        {
            if(tempLocation.CurrentWeatherDict != null)
            {
                //binding for current weather
                CurrentWeather cweather = new CurrentWeather
                    (
                        tempLocation.CurrentWeatherDict["temp"],
                        tempLocation.CurrentWeatherDict["temp_min"],
                        tempLocation.CurrentWeatherDict["temp_max"],
                        tempLocation.CurrentWeatherDict["pressure"],
                        tempLocation.CurrentWeatherDict["humidity"],
                        tempLocation.CurrentWeatherDict["main"],
                        tempLocation.CurrentWeatherDict["description"],
                        tempLocation.CurrentWeatherDict["speed"],
                        tempLocation.CurrentWeatherDict["deg"]
                    );
                TemperatureLB.DataContext = cweather;
                TemperatureminLB.DataContext = cweather;
                TemperaturemaxLB.DataContext = cweather;
                PressureLB.DataContext = cweather;
                HumidityLB.DataContext = cweather;
                MainLB.DataContext = cweather;
                DescriptionLB.DataContext = cweather;
                SpeedLB.DataContext = cweather;
                DegreeLB.DataContext = cweather;

            }

            if (tempLocation.FiveDayWeatherDict != null)
            {
                //binding dla 5 day weather
                treeView.Items.Clear();
                foreach (KeyValuePair<string, WConditions> wcondition in tempLocation.FiveDayWeatherDict)
                {
                    TreeViewItem treeItem = null;
                    treeItem = new TreeViewItem();
                    treeItem.Header = wcondition.Key;
                    Grid DynamicGrid = new Grid();
                    // DynamicGrid.ShowGridLines = true;
                    ColumnDefinition gridCol1 = new ColumnDefinition();
                    ColumnDefinition gridCol2 = new ColumnDefinition();
                    DynamicGrid.ColumnDefinitions.Add(gridCol1);
                    DynamicGrid.ColumnDefinitions.Add(gridCol2);
                    RowDefinition gridRow1 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow1);
                    RowDefinition gridRow2 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow2);
                    RowDefinition gridRow3 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow3);
                    RowDefinition gridRow4 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow4);
                    RowDefinition gridRow5 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow5);
                    RowDefinition gridRow6 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow6);
                    RowDefinition gridRow7 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow7);
                    RowDefinition gridRow8 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow8);
                    RowDefinition gridRow9 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow9);
                    RowDefinition gridRow10 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow10);
                    RowDefinition gridRow11 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow11);
                    RowDefinition gridRow12 = new RowDefinition();
                    DynamicGrid.RowDefinitions.Add(gridRow12);

                    //Temperature
                    TextBlock txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Temperature";
                    Grid.SetRow(txtBlock1, 0);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    TextBlock txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Temp.ToString();
                    Thickness margin = txtBlock1_1.Margin;
                    margin.Left = 30;
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 0);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Temperature Min
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Temperature (min)";
                    Grid.SetRow(txtBlock1, 1);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.TempMin.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 1);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Temperature Max
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Temperature (max)";
                    Grid.SetRow(txtBlock1, 2);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.TempMax.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 2);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Pressure
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Pressure";
                    Grid.SetRow(txtBlock1, 3);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Pressure.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 3);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Humidity
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Humidity";
                    Grid.SetRow(txtBlock1, 4);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Humidity.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 4);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Conditions
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Conditions";
                    Grid.SetRow(txtBlock1, 5);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Conditions.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 5);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Conditions Details
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Conditions details";
                    Grid.SetRow(txtBlock1, 6);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.ConditionsDesc.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 6);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Wind Speed
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Wind Speed";
                    Grid.SetRow(txtBlock1, 7);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.WindSpeed.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 7);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Wind Degree
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Wind Degree";
                    Grid.SetRow(txtBlock1, 8);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.WindDeg.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 8);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Clouds
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Clouds";
                    Grid.SetRow(txtBlock1, 9);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Cloud.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 9);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Rain
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Rain";
                    Grid.SetRow(txtBlock1, 10);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Rain.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 10);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    //Snow
                    txtBlock1 = null;
                    txtBlock1 = new TextBlock();
                    txtBlock1.Text = "Snow";
                    Grid.SetRow(txtBlock1, 11);
                    Grid.SetColumn(txtBlock1, 0);
                    DynamicGrid.Children.Add(txtBlock1);

                    txtBlock1_1 = null;
                    txtBlock1_1 = new TextBlock();
                    txtBlock1_1.Text = wcondition.Value.Snow.ToString();
                    txtBlock1_1.Margin = margin;
                    Grid.SetRow(txtBlock1_1, 11);
                    Grid.SetColumn(txtBlock1_1, 1);
                    DynamicGrid.Children.Add(txtBlock1_1);

                    treeItem.Items.Add(new TreeViewItem() { Header = DynamicGrid });
                    treeView.Items.Add(treeItem);
                }
            }

            
            
        }

        //private Dictionary<string, string> GetCurrentWeatherData(string ID, string apikey)
        //{
        //    request = (HttpWebRequest)WebRequest.Create(new Uri(string.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&APPID={1}&units=metric", ID, apikey)));
        //    AsyncCallback callback = new AsyncCallback(ProcessCurrentWeatherRequest);
        //    request.BeginGetResponse(callback, null);
        //    return currentWeatherDict;
        //}

        //private Dictionary<string, WConditions> GetFiveDayWeather(string ID, string apikey)
        //{

        //    //string URL = @"http://api.openweathermap.org/data/2.5/forecast?id=" + ID + @"&APPID=" + Setting.APPID + "&units=metric";
        //    request = (HttpWebRequest)WebRequest.Create(new Uri(string.Format("http://api.openweathermap.org/data/2.5/forecast?id={0}&APPID={1}&units=metric", ID, apikey)));
        //    AsyncCallback callback = new AsyncCallback(ProcessFiveDayWeatherRequest);
        //    request.BeginGetResponse(callback, null);
        //    return fiveDayWeatherDict;
        //}

        //private void ProcessCurrentWeatherRequest(IAsyncResult result)
        //{
        //    if (result.IsCompleted && request != null)
        //    {
        //        try
        //        {
        //            WebResponse response = request.EndGetResponse(result);
        //            using (Stream input = response.GetResponseStream())
        //            {
        //                var encoding = ASCIIEncoding.ASCII;
        //                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
        //                {
        //                    string responseText = reader.ReadToEnd();
        //                    WeatherApp.JSON_Classes.RootObject rootObject = JsonConvert.DeserializeObject<WeatherApp.JSON_Classes.RootObject>(responseText);

        //                    string temp = String.Empty;
        //                    string temp_min = String.Empty;
        //                    string temp_max = String.Empty;
        //                    string pressure = String.Empty;
        //                    string humidity = String.Empty;
        //                    string main = String.Empty;
        //                    string description = String.Empty;
        //                    string speed = String.Empty;
        //                    string deg = String.Empty;

        //                    //clear currentWeatherDict
        //                    currentWeatherDict.Clear();

        //                    #region Main
        //                    try { temp = rootObject.main.temp.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("temp", temp);
        //                    try { pressure = rootObject.main.pressure.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("pressure", pressure);
        //                    try { humidity = rootObject.main.humidity.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("humidity", humidity);
        //                    try { temp_min = rootObject.main.temp_min.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("temp_min", temp_min);
        //                    try { temp_max = rootObject.main.temp_max.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("temp_max", temp_max);
        //                    #endregion

        //                    #region Wind
        //                    try { speed = rootObject.wind.speed.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("speed", speed);
        //                    try { deg = rootObject.wind.deg.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("deg", deg);
        //                    #endregion

        //                    #region Weather
        //                    try { main = rootObject.weather[0].main.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("main", main);
        //                    try { description = rootObject.weather[0].description.ToString(); }
        //                    catch (NullReferenceException) { }
        //                    currentWeatherDict.Add("description", description);
        //                    #endregion

        //                }
        //            }
        //        }
        //        catch (WebException) { }
 
        //    }
        //}

        public async Task<Dictionary<string, string>> GetCurrentWeatherDataAsync(string ID, string apikey)
        {
            Dictionary<string, string> outputDict = await GetCurrentWeatherData(ID, apikey);  
            return outputDict;
        }

        private Task<Dictionary<string, string>> GetCurrentWeatherData(string ID, string apikey)
        {
            var outputDict = new Dictionary<string, string>();
            //Dictionary<string, string> outputDict = new Dictionary<string, string>();
            //string URL = @"http://api.openweathermap.org/data/2.5/weather?id=" + ID + @"&APPID=" + Setting.APPID + "&units=metric";
            string URL = $"http://api.openweathermap.org/data/2.5/weather?id={ID}&APPID={apikey}&units=metric";
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

                            return Task.FromResult<Dictionary<string, string>>(outputDict);
                        }

                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }
        }

        public async Task<Dictionary<string, WConditions>> GetFiveDayWeatherAsync(string ID, string apikey)
        {
            Dictionary<string, WConditions> outputDict = await GetFiveDayWeather(ID, apikey);
            return outputDict;
        }

        private Task<Dictionary<string, WConditions>> GetFiveDayWeather(string ID, string apikey)
        {
            Dictionary<string, WConditions> outputDict = new Dictionary<string, WConditions>();
            string URL = $"http://api.openweathermap.org/data/2.5/forecast?id={ID}&APPID={apikey}&units=metric";

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

                                try { dt = rootObject.list[i].dt.ToString(); } catch (NullReferenceException) { }
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
                                    dt, dt_txt, temp, temp_min, temp_max, pressure, humidity, main, description, all, speed, deg, rain, snow));
                            }

                            return Task.FromResult<Dictionary<string, WConditions>>(outputDict);

                        }
                    }
                }

            }
            catch (System.Net.WebException ex)
            {
                return null;
            }
        }

    //private Dictionary<string, string> GetCurrentWeatherData(string ID, string apikey)
    //    {

        //        Dictionary<string, string> outputDict = new Dictionary<string, string>();
        //        //string URL = @"http://api.openweathermap.org/data/2.5/weather?id=" + ID + @"&APPID=" + Setting.APPID + "&units=metric";
        //        string URL = $"http://api.openweathermap.org/data/2.5/weather?id={ID}&APPID={apikey}&units=metric";
        //        string json = String.Empty;

        //        HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
        //        request.UserAgent = "Googlebot/1.0 (googlebot@googlebot.com http://googlebot.com/)";

        //        if (!Setting.UseDefaultProxy)
        //        {
        //            WebProxy proxy = new WebProxy(Setting.ProxyURL, Setting.ProxyPort);
        //            request.Proxy = proxy;
        //        }
        //        else
        //        {
        //            IWebProxy proxy = WebRequest.GetSystemWebProxy();
        //            request.Proxy = proxy;
        //        }

        //        if (request.Proxy != null)
        //            request.Proxy.Credentials = CredentialCache.DefaultCredentials;

        //        try
        //        {
        //            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        //            {
        //                using (Stream responseStream = response.GetResponseStream())
        //                {
        //                    var encoding = ASCIIEncoding.ASCII;
        //                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
        //                    {
        //                        string responseText = reader.ReadToEnd();
        //                        WeatherApp.JSON_Classes.RootObject rootObject = JsonConvert.DeserializeObject<WeatherApp.JSON_Classes.RootObject>(responseText);

        //                        string temp = String.Empty;
        //                        string temp_min = String.Empty;
        //                        string temp_max = String.Empty;
        //                        string pressure = String.Empty;
        //                        string humidity = String.Empty;
        //                        string main = String.Empty;
        //                        string description = String.Empty;
        //                        string speed = String.Empty;
        //                        string deg = String.Empty;

        //                        outputDict.Add("found", "true");

        //                        #region Main
        //                        try { temp = rootObject.main.temp.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("temp", temp);
        //                        try { pressure = rootObject.main.pressure.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("pressure", pressure);
        //                        try { humidity = rootObject.main.humidity.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("humidity", humidity);
        //                        try { temp_min = rootObject.main.temp_min.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("temp_min", temp_min);
        //                        try { temp_max = rootObject.main.temp_max.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("temp_max", temp_max);
        //                        #endregion

        //                        #region Wind
        //                        try { speed = rootObject.wind.speed.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("speed", speed);
        //                        try { deg = rootObject.wind.deg.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("deg", deg);
        //                        #endregion

        //                        #region Weather
        //                        try { main = rootObject.weather[0].main.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("main", main);
        //                        try { description = rootObject.weather[0].description.ToString(); } catch (NullReferenceException) { }
        //                        outputDict.Add("description", description);
        //                        #endregion

        //                        return outputDict;
        //                    }

        //                }
        //            }
        //        }
        //        catch (System.Net.WebException ex)
        //        {
        //            return null;
        //        }

        //    }




        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateWeatherData();
        }

        private bool NetworkConnectionStatus(string urlStr)
        {
            try
            {
                Dns.GetHostEntry(urlStr);
                return true;
            }
            catch (SocketException ex)
            {
                return false;
            }
        }

        private void addLocationBTN_Click(object sender, RoutedEventArgs e)
        {
            LocationSearchWindow locationSearchWindow = new LocationSearchWindow(Setting);
            locationSearchWindow.LocationsList = LocationsList;
            locationSearchWindow.ShowDialog();
            this.UpdateListBox();
        }

        private void updateBTN_Click(object sender, RoutedEventArgs e)
        {
            UpdateWeatherData();
        }

        private void settingBTN_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow(Setting);
            settingWindow.ShowDialog();
            SetDispatcherTimerTNotifications();
        }

        private void exitBTN_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void removeLocationBTN_Click(object sender, RoutedEventArgs e)
        {
            if (this.LocationsLB.SelectedItem != null)
            {
                Location tempLocation = this.LocationsLB.SelectedItem as Location;
                MessageBoxResult result = MessageBox.Show("Do you want to remove location : " + tempLocation.Name + " ?",
                          "Location remove",
                          MessageBoxButton.YesNo,
                          MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    locationsList.Remove(locationsList.Where(i => i.Id == tempLocation.Id).Single());
                    UpdateListBox();
                }
            }
        }
    }

}
