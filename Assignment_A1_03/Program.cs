using Assignment_A1_03.Models;
using Assignment_A1_03.Services;
using static System.Net.WebRequestMethods;

namespace Assignment_A1_03;

public struct TextColor
{
    public const string NORMAL = "\x1b[39m";
    public const string RED = "\x1b[91m";
    public const string GREEN = "\x1b[92m";
    public const string YELLOW = "\x1b[93m";
    public const string BLUE = "\x1b[94m";
    public const string MAGENTA = "\x1b[95m";
    public const string CYAN = "\x1b[96m";
    public const string WHITE = "\x1b[97m";
    public const string BOLD = "\x1b[1m";
    public const string NOBOLD = "\x1b[22m";
    public const string UNDERLINE = "\x1b[4m";
    public const string NOUNDERLINE = "\x1b[24m";

    public static void Switch(bool switchColor)
    {
        if (switchColor)
        {
            Console.Write(WHITE);
        }
        else
        {
            Console.Write(CYAN);
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        OpenWeatherService service = new OpenWeatherService();

        //Your Code
        service.WeatherForecastAvailable += OnWeatherForecastAvailable;


        Task<Forecast>[] tasks = { null, null };
        Exception exception = null;
        try
        {
            double latitude = 60.67452;
            double longitude = 17.14174;

            //Create the two tasks and wait for comletion
            tasks[0] = service.GetForecastAsync(latitude, longitude);
            tasks[1] = service.GetForecastAsync("Stockholm");


            await Task.WhenAll(tasks[0], tasks[1]);

            tasks[0] = service.GetForecastAsync(latitude, longitude);
            tasks[1] = service.GetForecastAsync("Stockholm");

            await Task.WhenAll(tasks.ToArray());

            // For testing - it works! =)
            //await Task.Delay(TimeSpan.FromSeconds(65));

            //tasks[0] = service.GetForecastAsync(latitude, longitude);
            //tasks[1] = service.GetForecastAsync("Miami");

            //await Task.WhenAll(tasks[0], tasks[1]);
        }

        catch (Exception ex)
        {
            exception = ex;
            //How to handle an exception
            //Your Code
            Console.WriteLine($"{TextColor.RED}Weather Service error:{TextColor.NORMAL} {ex.Message}");
        }

        //Your Code
        foreach (Task<Forecast> task in tasks)
        {
            Console.WriteLine("-----------------------------");
            if (task != null && task.Status == TaskStatus.RanToCompletion)
                PresentForecast(task.Result);

        }
        service.WeatherForecastAvailable -= OnWeatherForecastAvailable;
    }

    public static void PresentForecast(Forecast forecast)
    {
        bool switchColor = false;

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
                   + $"Temp: {timeStamp.Temp,6:F2}°C, "
                   + $"Vind: {timeStamp.Wind,5:F2} m/s");
                switchColor = !switchColor;
            }
            Console.WriteLine($"{TextColor.NORMAL}");
        }

        //Console.WriteLine("Press any key to continue...");
        //Console.ReadKey();
    }

    //Event handler declaration
    public static void OnWeatherForecastAvailable(object sender, string message)
    {

        Console.WriteLine($"{TextColor.GREEN}Message from Weather Service:{TextColor.NORMAL} {message}");

    }
}
