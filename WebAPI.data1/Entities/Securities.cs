using Newtonsoft.Json;

namespace WebAPI.data1.Entities
{
    public class Securities
    {
        [JsonProperty(PropertyName = "Ticker.symbol")]
        public string symbol { get; set; }
        public string Security { get; set; }
        public string Sector { get; set; }
        public string SubIndustry { get; set; }
    }
}