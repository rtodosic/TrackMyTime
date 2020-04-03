using Newtonsoft.Json;
using System;

namespace TrackMyTime.Models
{
    public class TimeModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }


        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        
        [JsonProperty(PropertyName = "timeGroup")]
        public string TimeGroup { get; set; }

        
        [JsonProperty(PropertyName = "start")]
        public DateTime? Start { get; set; }

        
        [JsonProperty(PropertyName = "end")]
        public DateTime? End { get; set; }

        
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
    }
}
