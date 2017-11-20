using Newtonsoft.Json;

namespace Uspator.Model
{
    public class GameInfo
    {
        [JsonProperty("edgeLength")]
        public int EdgeLength { get; set; }
        
        [JsonProperty("numOfBotsInPlay")]
        public int NumOfBotsInPlay { get; set; }
        
        [JsonProperty("currentTick")]
        public int CurrentTick { get; set; }
        
        [JsonProperty("numOfTasksPerTick")]
        public int NumOfTasksPerTick { get; set; }
    }
}