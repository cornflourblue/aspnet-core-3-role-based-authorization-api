using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Authenticate model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest createUserRequest)
        {
            var user = _userService.Create(createUserRequest);

            if (user == null)
                return BadRequest(new { message = "Something went wrong!" });

            return Ok(new { Data = user } );
        }

        //[Authorize(Roles = Role.Admin)]
        [AllowAnonymous]
        [HttpPost("attachrole")]
        public IActionResult AttachRole([FromBody] AttachRoleRequest attachRoleRequest)
        {
            var response = _userService.AttachRole(attachRoleRequest);

            return Ok(response);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("update")]
        public IActionResult Update([FromBody] UpdateUserRequest updateUserRequest)
        {
            var user = _userService.Update(updateUserRequest);

            if (user == null)
                return BadRequest(new { message = "Something went wrong!" });

            return Ok(new { Data = user });
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Delete(id);

            if (user == null)
                return BadRequest(new { message = "Something went wrong!" });

            return Ok(new { Data = user });
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            // only allow admins to access other user records
            //var currentUserId = int.Parse(User.Identity.Name);
            //if (id != currentUserId && !User.IsInRole(Role.Admin))
            //    return Forbid();

            var user =  _userService.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
