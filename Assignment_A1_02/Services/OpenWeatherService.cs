using Assignment_A1_02.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Assignment_A1_02.Services;
public class OpenWeatherService
{
    readonly HttpClient _httpClient = new HttpClient();
    readonly string _apiKey = "e0907403b9e636533faefbfe0d854a7b"; // Replace with your OpenWeatherMap API key

    private readonly ConcurrentDictionary<string, (Forecast forecast, DateTime fetched)> _cache = new();
    private static readonly TimeSpan CachedTime = TimeSpan.FromMinutes(1);

    //Event declaration
    public event EventHandler<string>? WeatherForecastAvailable;
    protected virtual void OnWeatherForecastAvailable(string message)
    {
        WeatherForecastAvailable?.Invoke(this, message);
    }
    public async Task<Forecast> GetForecastAsync(string City)
    {
        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={_apiKey}";

        if (_cache.TryGetValue(uri, out var cached) && DateTime.UtcNow - cached.fetched < CachedTime)
        {
            OnWeatherForecastAvailable($"Cached weather forecast for {City} available.");
            return cached.forecast;
        }

        Forecast forecast = await ReadWebApiAsync(uri);

        //Event code here to fire the event
        //Your code
        if (forecast != null)
        {
            _cache[uri] = (forecast, DateTime.UtcNow);
            OnWeatherForecastAvailable($"New weather forecast for {City} downloaded.");
            
        }
        return forecast;
    }
    public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
    {
        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={_apiKey}";

        if (_cache.TryGetValue(uri, out var cached) && DateTime.UtcNow - cached.fetched < CachedTime)
        {
            OnWeatherForecastAvailable($"Cached weather forecast for ({latitude}, {longitude}) avaiable.");
            return cached.forecast;
        }
        
        Forecast forecast = await ReadWebApiAsync(uri);

        //Event code here to fire the event
        //Your code
        if (forecast != null)
        {
            _cache[uri] = (forecast, DateTime.UtcNow);
            OnWeatherForecastAvailable($"New weather forecast for ({latitude}, {longitude}) downloaded.");
        }

        return forecast;
    }
    private async Task<Forecast> ReadWebApiAsync(string uri)

    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        WeatherApiData wd = JsonConvert.DeserializeObject<WeatherApiData>(content);

        //Convert WeatherApiData to Forecast using Linq.
        //Your code
        var forecast = new Forecast
        {
            City = wd.city.name,
            Items = wd.list.Select(w => new ForecastItem
            {
                DateTime = UnixTimeStampToDateTime(w.dt),
                Temperature = w.main.temp,
                WindSpeed = w.wind.speed,
                Description = w.weather.First().description,
                Icon = $"http://openweathermap.org/img/w/{w.weather.First().icon}.png"
            }).ToList()
        };
        return forecast;
    }

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp) => DateTime.UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
}

