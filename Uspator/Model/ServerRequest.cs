using System.Collections.Generic;
using Newtonsoft.Json;

namespace Uspator.Model
{
    public class ServerRequest
    {
        [JsonProperty("currentPlayer")]
        public CurrentPlayer CurrentPlayer { get; set; }
        
        [JsonProperty("gameInfo")]
        public GameInfo GameInfo { get; set; }
        
        [JsonProperty("players")]
        public List<Player> Players { get; set; }
        
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
}