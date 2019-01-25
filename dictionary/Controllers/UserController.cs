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
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("/register")]
        public async Task<ActionResult<User>> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {

            if (await _unitOfWork._authRepository.UserExits(userForRegisterDTO.Username))
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

           
            var createdUser = await _unitOfWork._authRepository.Register(userToCreate, userForRegisterDTO.Password);
            return await Task.FromResult(Ok(createdUser));

        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
        

    }
}