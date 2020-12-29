using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Response
    {
        [JsonProperty("data", Required = Required.Always)]
        public object Data { get; set; } 
        
        [JsonProperty("message")]
        public string Message { get; set; }

    }
}