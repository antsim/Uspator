using Newtonsoft.Json;

namespace Uspator.Model
{
    public class BombTask : TaskBase
    {
        [JsonProperty("task")]
        public override string Task => "BOMB";
        
        [JsonProperty("x")]
        public int X { get; set; }
        
        [JsonProperty("y")]
        public int Y { get; set; }
        
        [JsonProperty("z")]
        public int Z { get; set; }
    }
}