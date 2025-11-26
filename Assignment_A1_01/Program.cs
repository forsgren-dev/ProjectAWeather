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
            .Select(group => new
            {
                Date = group.Key,
                Items = group.Select(item => new
                {
                    Time = item.DateTime.ToString("HH:mm"),
                    Desc = item.Description,
                    Temp = item.Temperature,
                    Wind = item.WindSpeed
                })
            });

        Console.WriteLine(
            $"{TextColor.BOLD}{TextColor.UNDERLINE}{TextColor.YELLOW}" +
            $"Väderprognos för {forecast.City}:\n" +
            $"{TextColor.NOBOLD}{TextColor.NOUNDERLINE}");

        foreach (var date in forecastByDate)
        {
            Console.WriteLine($"{TextColor.YELLOW}{date.Date.ToShortDateString()}{TextColor.NORMAL}");

            foreach (var timeStamp in date.Items)
            {
                TextColor.Switch(switchColor);
                Console.WriteLine($"- ".PadLeft(5)
                   + $"{timeStamp.Time}: "
                   + $"{char.ToUpper(timeStamp.Desc[0]) + timeStamp.Desc[1..],-20}"
                   + $"Temp: {timeStamp.Temp, 6:F2}°C, "
                   + $"Vind: {timeStamp.Wind, 5:F2} m/s");
                switchColor = !switchColor;
            }
            Console.WriteLine($"{TextColor.NORMAL}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

    }
}

