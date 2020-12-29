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
    public interface IUserService
    {
        User Authenticate(string username, string password);
        List<IdentityUser> GetAll();
        IdentityUser GetById(string id);
        Task<Response> Create(CreateUserRequest createUserRequest);
        Task<Response> Update(UpdateUserRequest updateUserRequest);
        Task<Response> Delete(string id);
        Task<Response> AttachRole(AttachRoleRequest attachRoleRequest);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<User> _users = new List<User>
        //{ 
        //    new User { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", Password = "admin", Role = Role.Admin },
        //    new User { Id = 2, FirstName = "Normal", LastName = "User", Username = "user", Password = "user", Role = Role.User } 
        //};

        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _applicationDbContext;

        public UserService(IOptions<AppSettings> appSettings, ApplicationDbContext applicationDbContext)
        {
            _appSettings = appSettings.Value;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Response> Create(CreateUserRequest createUserRequest)
        {
            try
            {
                var dbUser = _applicationDbContext.Users.SingleOrDefault<IdentityUser>(user => user.UserName == createUserRequest.Username && user.PasswordHash == createUserRequest.Password);
                if (dbUser != null)
                {
                    return new Response { Data = dbUser };
                }
                var user = new IdentityUser
                {
                    UserName = createUserRequest.Username,
                    Email = createUserRequest.Email,
                    PasswordHash = createUserRequest.Password,
                };
                _applicationDbContext.Users.Add(user);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = user };
            }
            catch (Exception e)
            {
                return new Response { Message = e.Message };
            }
        }

        public async Task<Response> Update(UpdateUserRequest updateUserRequest)
        {
            try
            {
                var dbUser = _applicationDbContext.Users.SingleOrDefault<IdentityUser>(user => user.UserName == updateUserRequest.Username && user.PasswordHash == updateUserRequest.Password);
                if (dbUser == null)
                {
                    return new Response { Message = "User not found!" };
                }
                dbUser.UserName = updateUserRequest.Username;
                dbUser.Email = updateUserRequest.Email;
                dbUser.PasswordHash = updateUserRequest.Password;

                _applicationDbContext.Users.Update(dbUser);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = dbUser, Message = "User updated successfully!" };
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
                var dbUser = _applicationDbContext.Users.SingleOrDefault<IdentityUser>(user => user.Id == id);
                if (dbUser == null)
                {
                    return new Response { Message = "User not found!" };
                }
                _applicationDbContext.Users.Remove(dbUser);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = dbUser, Message = "User removed successfully!" };
            }
            catch (Exception e)
            {
                return new Response { Message = e.Message };
            }
        }

        public User Authenticate(string username, string password)
        {
            //var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);
            var dbUser = _applicationDbContext.Users.SingleOrDefault(user => user.UserName == username && user.PasswordHash == password);

            // return null if user not found
            if (dbUser == null)
                return null;

            var dbUserRole = _applicationDbContext.UserRoles.SingleOrDefault(userRole => userRole.UserId == dbUser.Id);


            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claims = new Claim[] { new Claim(ClaimTypes.Name, dbUser.Id.ToString()) };
            if (dbUserRole != null)
            {
                var dbRole = _applicationDbContext.Roles.SingleOrDefault(role => role.Id == dbUserRole.RoleId);
                claims.Prepend(new Claim(ClaimTypes.Role, dbRole.Name));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new Claim[] 
                //{
                //    new Claim(ClaimTypes.Name, dbUser.Id.ToString()),
                //    new Claim(ClaimTypes.Role, user.Role)
                //}),
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //user.Token = tokenHandler.WriteToken(token);

            return new User
            {
                Id = dbUser.Id,
                Username = dbUser.UserName,
                Token = tokenHandler.WriteToken(token),
            };
        }

        public List<IdentityUser> GetAll()
        {
            var users = _applicationDbContext.Users.ToList();
            return users;
            //return _users.WithoutPasswords();
        }

        public IdentityUser GetById(string id) 
        {
            var dbUser = _applicationDbContext.Users.SingleOrDefault<IdentityUser>(user => user.Id == id);
            if (dbUser == null)
            {
                return null;
            }
            return dbUser;
        }

        public async Task<Response> AttachRole(AttachRoleRequest attachRoleRequest)
        {
            Console.WriteLine("----ATTACH ROLE----");
            try
            {
                var dbUser = _applicationDbContext.Users.SingleOrDefault<IdentityUser>(user => user.Id == attachRoleRequest.UserId);
                var dbRole = _applicationDbContext.Roles.SingleOrDefault<IdentityRole>(role => role.Name == attachRoleRequest.RoleName);
                if (dbUser == null)
                {
                    return new Response { Message = "User not found!" };
                }
                if (dbRole == null)
                {
                    return new Response { Message = "Role not found!" };
                }
                var newUserRole = new IdentityUserRole<string>
                {
                    UserId = dbUser.Id,
                    RoleId = dbRole.Id
                };
                var dbUserRole = _applicationDbContext.UserRoles.SingleOrDefault<IdentityUserRole<string>>(userRole => userRole.UserId == dbUser.Id && userRole.RoleId == dbRole.Id);
                if (dbUserRole != null)
                {
                    return new Response { Data = new { User = dbUser, Role = dbRole }, Message = "Role already attached to this user" };
                }
                _applicationDbContext.UserRoles.Add(newUserRole);
                await _applicationDbContext.SaveChangesAsync();
                return new Response { Data = new {
                    User = dbUser,
                    Role = dbRole,
                } };
            } catch (Exception e)
            {
                return new Response { Message = e.Message };
            }
        }
    }
}