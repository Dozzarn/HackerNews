using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        public UserController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {

            if (await _authRepository.UserExits(userForRegisterDTO.Username))
            {
                ModelState.AddModelError("Username", "Username Already Exists");
            }
            if (userForRegisterDTO.Password != userForRegisterDTO.PasswordAgain)
            {
                ModelState.AddModelError("Password", "Password Not Matched");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userToCreate = new User
            {
                Username = userForRegisterDTO.Username,
            };

           
            var createdUser = await _authRepository.Register(userToCreate, userForRegisterDTO.Password);
            return await Task.FromResult(Ok(createdUser));

        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
        

    }
}