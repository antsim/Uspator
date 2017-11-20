using Newtonsoft.Json;

namespace Uspator.Model
{
    public class Player
    {
        [JsonProperty("Name")]
        public string name { get; set; }
        
        [JsonProperty("x")]
        public int X { get; set; }
        
        [JsonProperty("y")]
        public int Y { get; set; }
        
        [JsonProperty("z")]
        public int Z { get; set; }
    }
}