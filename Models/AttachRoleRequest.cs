using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class AttachRoleRequest
    {
        [JsonProperty("userId", Required = Required.Always)]
        public string UserId { get; set; } 
        
        [JsonProperty("roleName", Required = Required.Always)]
        public string RoleName { get; set; }
    }
}