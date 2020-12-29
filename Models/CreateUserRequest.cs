using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class CreateUserRequest
    {
        [JsonProperty("username", Required = Required.Always)]
        public string Username { get; set; } 
        
        [JsonProperty("email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty("password", Required = Required.Always)]
        public string Password { get; set; }
    }
}