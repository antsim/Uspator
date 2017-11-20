using Newtonsoft.Json;

namespace Uspator.Model
{
    public class MoveTask : TaskBase
    {
        [JsonProperty("task")]
        public override string Task => "MOVE";
        
        [JsonProperty("direction")]
        public string Direction { get; set; }
    }
}