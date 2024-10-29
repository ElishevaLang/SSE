using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace TimeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SseController : ControllerBase
    {
        [HttpGet("time")]
        public async Task GetTimeUpdates()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");

            var timeZones = new[]
            {
                new { Country = "USA (New York)", TimeZoneId = "Eastern Standard Time" },
                new { Country = "Switzerland", TimeZoneId = "W. Europe Standard Time" },
                new { Country = "England", TimeZoneId = "GMT Standard Time" },
                new { Country = "Israel", TimeZoneId = "Israel Standard Time" }
            };

            while (true)
            {
                var times = new List<object>();

                foreach (var timeZone in timeZones)
                {
                    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone.TimeZoneId);
                    var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);

                    times.Add(new
                    {
                        Country = timeZone.Country,
                        Time = currentTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                    });
                }

                var json = JsonConvert.SerializeObject(times);
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"data: {json}\n\n"));
                await Response.Body.FlushAsync();

                // Update every second
                await Task.Delay(1000);
            }
        }
    }
}
