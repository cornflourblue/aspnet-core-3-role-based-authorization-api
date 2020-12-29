using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class CreateRoleRequest
    {
        [JsonProperty("roleName", Required = Required.Always)]
        public string RoleName { get; set; } 
    }
}