using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UpdateUserRequest
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; } 
        
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}