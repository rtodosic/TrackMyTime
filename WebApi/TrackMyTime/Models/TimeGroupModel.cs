using Newtonsoft.Json;

namespace TrackMyTime.Models
{
    public class TimeGroupModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }


        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }


        [JsonProperty(PropertyName = "name")]

        public string Name { get; set; }
    }
}
