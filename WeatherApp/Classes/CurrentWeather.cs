using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WeatherApp.Classes
{
    public class CurrentWeather : INotifyPropertyChanged
    {
        /*
         Temp (min)
Temp (max)
Pressure (jednostki)
Humidity (jednostki)

Conditions: Main: description

Wind:
Speed: xx Degree: xx
             */
        private string _temperature;
        private string _temperaturemin;
        private string _temperaturemax;
        private string _pressure;
        private string _humidity;
        private string _main;
        private string _description;
        private string _speed;
        private string _degree;


        public string Temperature
        {
            get { return _temperature; }

            set
            {
                if (_temperature == value) return;
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public string Temperaturemin
        {
            get{return _temperaturemin; }

            set
            {
                if (_temperaturemin == value) return;
                _temperaturemin = value;
                OnPropertyChanged("Temperaturemin");
            }
        }

        public string Temperaturemax
        {
            get { return _temperaturemax; }

            set
            {
                if (_temperaturemax == value) return;
                _temperaturemax = value;
                OnPropertyChanged("Temperaturemax");
            }
        }

        public string Pressure
        {
            get { return _pressure; }

            set
            {
                if (_pressure == value) return;
                _pressure = value;
                OnPropertyChanged("Pressure");
            }
        }

        public string Humidity
        {
            get { return _humidity; }

            set
            {
                if (_humidity == value) return;
                _humidity = value;
                OnPropertyChanged("Humidity");
            }
        }

        public string Main
        {
            get { return _main; }

            set
            {
                if (_main == value) return;
                _main = value;
                OnPropertyChanged("Main");
            }
        }

        public string Description
        {
            get { return _description; }

            set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Speed
        {
            get { return _speed; }

            set
            {
                if (_speed == value) return;
                _speed = value;
                OnPropertyChanged("Speed");
            }
        }

        public string Degree
        {
            get { return _degree; }

            set
            {
                if (_degree == value) return;
                _degree = value;
                OnPropertyChanged("Degree");
            }
        }
        

        public CurrentWeather(
            string temperature,
            string temperaturemin,
            string temperaturemax,
            string pressure,
            string humidity,
            string main,
            string description,
            string speed,
            string degree
            )
        {
            this.Temperature = temperature;
            this.Temperaturemin = temperaturemin;
            this.Temperaturemax = temperaturemax;
            this.Pressure = pressure;
            this.Humidity = humidity;
            this.Main = main;
            this.Description = description;
            this.Speed = speed;
            this.Degree = degree;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
