using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebAPI.Database;

namespace WebApi.Services
{
    public interface IRoleService
    {
        IdentityRole GetById(string id);
        List<IdentityRole> GetAll();
        Task<Response> Create(CreateRoleRequest createRoleRequest);
        Task<Response> Update(UpdateRoleRequest updateRoleRequest);
        Task<Response> Delete(string id);
    }

    public class RoleService : IRoleService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _applicationDbContext;

        public RoleService(IOptions<AppSettings> appSettings, ApplicationDbContext applicationDbContext)
        {
            _appSettings = appSettings.Value;
            _applicationDbContext = applicationDbContext;
        }

        public List<IdentityRole> GetAll()
        {
            return _applicationDbContext.Roles.ToList();
        }

        public async Task<Response> Create(CreateRoleRequest createRoleRequest)
        {
            try
            {
                var dbRole = _applicationDbContext.Roles.SingleOrDefault<IdentityRole>(role => role.Name == createRoleRequest.RoleName);
                if (dbRole != null)
                {
                    return new Response { Data = dbRole };
                }
                var newRole = new IdentityRole
                {
                    Name = createRoleRequest.RoleName,
                };
                _applicationDbContext.Roles.Add(newRole);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = newRole };
            }
            catch (Exception e)
            {
                return new Response { Message = e.Message };
            }
        }

        public async Task<Response> Update(UpdateRoleRequest updateRoleRequest)
        {
            try
            {
                var dbRole = _applicationDbContext.Roles.SingleOrDefault<IdentityRole>(role => role.Name == updateRoleRequest.RoleName);
                if (dbRole == null)
                {
                    return new Response { Message = "User not found!" };
                }
                dbRole.Name = updateRoleRequest.RoleName;

                _applicationDbContext.Roles.Update(dbRole);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = dbRole, Message = "Role updated successfully!" };
            }
            catch (Exception e)
            {
                return new Response { Message = e.Message };
            }
        }
        
        public async Task<Response> Delete(string id)
        {
            try
            {
                var dbRole = _applicationDbContext.Roles.SingleOrDefault<IdentityRole>(role => role.Id == id);
                if (dbRole == null)
                {
                    return new Response { Message = "Role not found!" };
                }
                _applicationDbContext.Roles.Remove(dbRole);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = dbRole, Message = "User removed successfully!" };
            }
            catch (Exception e)
            {
                return new Response { Message = e.Message };
            }
        }

        public IdentityRole GetById(string id) 
        {
            var dbRole = _applicationDbContext.Roles.SingleOrDefault<IdentityRole>(role => role.Id == id);
            if (dbRole == null)
            {
                return null;
            }
            return dbRole;
        }
    }
}