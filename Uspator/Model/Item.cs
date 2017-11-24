using Newtonsoft.Json;

namespace Uspator.Model
{
    public class Item
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("x")]
        public int X { get; set; }
        
        [JsonProperty("y")]
        public int Y { get; set; }
        
        [JsonProperty("z")]
        public int Z { get; set; }
        
        public string GetPositionString()
        {
            return $"{X}{Y}{Z}";
        }
    }
}