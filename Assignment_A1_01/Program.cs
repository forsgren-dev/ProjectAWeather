using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01;

class Program
{
    static async Task Main(string[] args)
    {
        double latitude = 59.5086798659495;
        double longitude = 18.2654625932976;

        Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

        //Your Code to present each forecast item in a grouped list
        Console.WriteLine($"Weather forecast for {forecast.City}");
        var forecastByDate = forecast.Items
            .GroupBy(item => item.DateTime.Date)
            .OrderBy(group => group.Key)
            .SelectMany(group => 
            {
                var date = group.Key;
                return group.Select(item => new 
                {
                    Date = date,
                    Time = item.DateTime.ToString("HH:mm"),
                    Temperature = item.Temperature,
                    Description = item.Description,
                    Wind = item.WindSpeed
                });
            });

            foreach (var dateGroup in forecastByDate.GroupBy(g => g.Date))
            {
                Console.WriteLine($"{dateGroup.Key.ToShortDateString()}");
                foreach (var timeStamp in dateGroup)
                {
                    Console.WriteLine($"- ".PadLeft(5) 
                        + $"{timeStamp.Time}: " 
                        + $"{timeStamp.Description, -20}" 
                        + $"Temp: {timeStamp.Temperature,6}°C,"
                        + $"Wind: {timeStamp.Wind} m/s");
                }
        }
        
    }
}

