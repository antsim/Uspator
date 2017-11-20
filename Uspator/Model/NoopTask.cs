using Newtonsoft.Json;

namespace Uspator.Model
{
    public class NoopTask : TaskBase
    {
        [JsonProperty("task")]
        public override string Task => "NOOP";
    }
}