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

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string temp = String.Empty;
            string pressure = String.Empty;
            string humidity = String.Empty;
            string temp_min = String.Empty;
            string temp_max = String.Empty;
            string speed = String.Empty;
            string deg = String.Empty;
            string id = String.Empty;
            string main = String.Empty;
            string description = String.Empty;
            string icon = String.Empty;
            string type = String.Empty;
            string set_id = String.Empty;
            string message = String.Empty;
            string country = String.Empty;
            string sunrise = String.Empty;
            string sunset = String.Empty;

            string URL = @"http://api.openweathermap.org/data/2.5/weather?q=Belleville&APPID=53849b8462e783dd24f9bdfb43563129&units=metric";
            string json = String.Empty;

            WebRequest request = WebRequest.Create(URL);
            request.Credentials = CredentialCache.DefaultCredentials;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var encoding = ASCIIEncoding.ASCII;
                    //StreamReader reader = new StreamReader(responseStream);
                    
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                    {
                        string responseText = reader.ReadToEnd();
                        WeatherApp.JSON_Classes.RootObject rootObject = JsonConvert.DeserializeObject<WeatherApp.JSON_Classes.RootObject>(responseText);

                        #region Main
                        temp = rootObject.main.temp.ToString();
                        pressure = rootObject.main.temp.ToString();
                        humidity = rootObject.main.humidity.ToString();
                        temp_min = rootObject.main.temp_min.ToString();
                        temp_max = rootObject.main.temp_max.ToString();
                        #endregion

                        #region Wind
                        speed = rootObject.wind.speed.ToString();
                        deg = rootObject.wind.deg.ToString();
                        #endregion

                        #region Weather
                        foreach (var item in rootObject.weather)
                        {
                            description = item.description.ToString();
                            id = item.id.ToString();
                            main = item.main.ToString();
                            icon = item.icon.ToString();
                            //Console.WriteLine(string.Join(", ", item.id) + " " + string.Join(", ", item.name));
                        }
                        #endregion

                        //sys_id = rootObject.sys
                        //message = String.Empty;
                        //country = String.Empty;
                        //sunrise = String.Empty;
                        //sunset = String.Empty;

                    }
                    //MessageBox.Show(reader.ReadToEnd());



                }
            }


        }

        void DoIt(string url)
        {
            HttpWebRequest httpWebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;

            using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Server error (HTTP {0}: {1}).",
                        httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                }


                //Stream stream = httpWebResponse.GetResponseStream();

                //DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(InputGpioPort));
                //InputGpioPort objResponse = (InputGpioPort)dataContractJsonSerializer.ReadObject(stream);
                //if (objResponse == null)
                //{
                //    Debug.WriteLine("got nothing!");
                //}
                //else
                //{
                //    Debug.WriteLine(String.Format("pin: {0}, gpio: {1}, value: {2}",
                //        (objResponse).pin, (objResponse).gpio,
                //        (objResponse).value));
                //}
            }

        }

    }
}
