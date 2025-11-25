using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01;

class Program
{
    static async Task Main(string[] args)
    {
        double latitude = 60.67452;
        double longitude = 17.14174;
        bool switchColor = false;
        
        Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

        //Your Code to present each forecast item in a grouped list

        var forecastByDate = forecast.Items
            .GroupBy(item => item.DateTime.Date)
            .OrderBy(group => group.Key)
            .SelectMany(group =>
            {
                return group.Select(item => new
                {
                    Date = group.Key,
                    Time = item.DateTime.ToString("HH:mm"),
                    Temp = item.Temperature,
                    Desc = item.Description,
                    Wind = item.WindSpeed
                });
            });

        Console.WriteLine(
            $"{TextColor.BOLD}{TextColor.UNDERLINE}" +
            $"Weather forecast for {forecast.City}:\n" +
            $"{TextColor.NOBOLD}{TextColor.NOUNDERLINE}");

        foreach (var date in forecastByDate.GroupBy(g => g.Date))
        {
            Console.WriteLine($"{TextColor.YELLOW}{date.Key.ToShortDateString()}{TextColor.NORMAL}");
            foreach (var timeStamp in date)
            {
                TextColor.Switch(switchColor);
                Console.WriteLine($"- ".PadLeft(5)
                   + $"{timeStamp.Time}: "
                   + $"{timeStamp.Desc,-20}"
                   + $"Temp: {timeStamp.Temp,6}°C, "
                   + $"Wind: {timeStamp.Wind,-4} m/s");
                switchColor = !switchColor;
            }
            Console.WriteLine($"{TextColor.NORMAL}");
        }
        


    }
}

