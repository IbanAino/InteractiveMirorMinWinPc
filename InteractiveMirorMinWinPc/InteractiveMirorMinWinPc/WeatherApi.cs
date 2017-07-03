using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace InteractiveMirorMinWinPc
{
    class WeatherApi
    {
        // ATTRIBUTS
        string weatherApiKey = "dfc0a483214d12e4361e5035e1d6f5d9";

        //CONSTRUCTOR
        public WeatherApi()
        {

        }


        // METHODS
        public string checkWeather (int cityId)
        {
            JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&q={1}", weatherApiKey, "id=3034473")));

            return jsonData.ToString();
        }
    }
}
