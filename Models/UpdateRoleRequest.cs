using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UpdateRoleRequest
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("roleName", Required = Required.Always)]
        public string RoleName { get; set; } 
    }
}