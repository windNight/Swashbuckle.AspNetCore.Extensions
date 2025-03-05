using System.Attributes;
using Microsoft.AspNetCore.Mvc;
using SwaggerDemo_Net8.@internal;

namespace SwaggerDemo_Net8.Controllers
{
    [Route("api/test")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly Dictionary<string, string> SignDict = new()
        {
            { "Authorization", "格式 Bearer xx" },
            { "AppId", "AppId" },
            { "AppCode", "AppCode" },
            { "AppToken", "Sign" },
            { "H1", "H1" },
            { "Ts", "当前时间戳" },
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        private string GetHeaderData(HttpRequest httpRequest, string headerName)
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return string.Empty;
        }


        [HttpGet("tt")]
        [NonAuth]
        public object Get()
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            return new { signData, rangeData };
        }

        [HttpGet("t22")]
        [ProducesResponseType(typeof(WeatherForecast), 10210)]
        public TestInput Get11122([FromQuery] TestInput input)
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            return input;
        }

        [HttpGet("t")]
        [ProducesResponseType(typeof(WeatherForecast), 10210)]
        public object Get111([FromQuery] TestInput input)
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            return new { signData, rangeData };
        }


        [HttpGet("tt1")]
        public IEnumerable<WeatherForecast> Get([FromQuery] TestInput input)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
        }

        [HttpGet("rr2")]
        public IEnumerable<WeatherForecast2> Get2([FromQuery] WeatherForecast2 input)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast2
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
        }


        [HttpGet("hiddenapi/testapi")]
        [HiddenApi(TestApi = true)]
        public IEnumerable<WeatherForecast2> TestApi([FromQuery] WeatherForecast2 input)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast2
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
        }


        [HttpGet("hiddenapi/sysapi")]
        [HiddenApi(TestApi = false, SysApi = true)]
        public IEnumerable<WeatherForecast2> SysApi([FromQuery] WeatherForecast2 input)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast2
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
        }

        [HttpGet("hiddenapi")]
        [HiddenApi(TestApi = false)]
        public IEnumerable<WeatherForecast2> HiddenApi([FromQuery] WeatherForecast2 input)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast2
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
        }
    }
}
