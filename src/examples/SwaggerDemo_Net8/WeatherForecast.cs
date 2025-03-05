namespace SwaggerDemo_Net8
{
    public class TestInput
    {
        public int TemperatureC { get; set; } = 0;
        public string SummaryDataDate { get; set; } = "";

    }
    public class WeatherForecast
    {
        /// <summary> summary Date  </summary>
        public DateOnly Date { get; set; } = DateOnly.MinValue;

        /// <summary> summary TemperatureC  </summary>
        public int TemperatureC { get; set; } = 0;

        /// <summary> summary TemperatureF  </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary> summary Summary  </summary>
        public string? Summary { get; set; } = "";

        /// <summary> summary SummaryDataDate  </summary>
        public string SummaryDataDate { get; set; } = "";
    }


    [Obsolete("xxx")]
    public class WeatherForecast2
    {
        /// <summary> summary Date  </summary>
        public DateOnly Date { get; set; }

        /// <summary> summary TemperatureC  </summary>
        public int TemperatureC { get; set; }

        /// <summary> summary TemperatureF  </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary> summary Summary  </summary>
        public string? Summary { get; set; }

        /// <summary> summary SummaryDataDate  </summary>
        public string SummaryDataDate { get; set; }
    }
}
