using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///  Register 
        /// </summary>
        /// <param name="userForRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<UserForRegisterResultDTO>> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {
            UserForRegisterResultDTO result;

            if (await _unitOfWork._authRepository.UserExits(userForRegisterDTO.Username))
            {
                result = new UserForRegisterResultDTO
                {
                    Username = null,
                    Status = false,
                    StatusInfoMessage = "Username Already Exists"
                };
                return await Task.FromResult(Ok(result));


            }
            if (userForRegisterDTO.Password != userForRegisterDTO.PasswordAgain)
            {
                result = new UserForRegisterResultDTO
                {
                    Username = null,
                    Status = false,
                    StatusInfoMessage = "Passwords Not Matched"
                };
                return await Task.FromResult(Ok(result));
            }
            if (!ModelState.IsValid)
            {
                return await Task.FromResult(Ok(ModelState));
            }
            var userToCreate = new User
            {
                Username = userForRegisterDTO.Username,
            };

            var createdUser = await _unitOfWork._authRepository.Register(userToCreate, userForRegisterDTO.Password);
            if (createdUser != null)
            {
                _unitOfWork.Commit();
                _unitOfWork.Dispose();
                result = new UserForRegisterResultDTO
                {
                    Username = createdUser.Username,
                    Status = true,
                    StatusInfoMessage = "Kullanıcı kaydı başarıyla tamamlandı"
                };
                return await Task.FromResult(Ok(result));
            }
            result = new UserForRegisterResultDTO
            {
                Username = null,
                Status = false,
                StatusInfoMessage = "Kayıt işlemi sırasında bir sıkıntıyla karşılaşıldı"
            };
            return await Task.FromResult(Ok(result));


        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userForLoginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserForLoginResultDTO>> Login([FromBody]UserForLoginDTO userForLoginDTO)
        {
            var user = await _unitOfWork._authRepository.Login(userForLoginDTO.Username, userForLoginDTO.Password);
            if (user == null)
            {
                return await Task.FromResult(Unauthorized());
            }

            return await Task.FromResult(Ok(TokenHandler(user,userForLoginDTO.RememberMe)));
        }



        public async Task<UserForLoginResultDTO> TokenHandler(User user, bool rememberMe)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_unitOfWork._configuration.GetSection("AppSettings:Token").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                     new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                     new Claim(ClaimTypes.Name,user.Username)


                }),
                Expires = rememberMe ? DateTime.Now.AddDays(365)  : DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return await Task.FromResult(new UserForLoginResultDTO
            {
                Token = tokenString,
                Status = true,
                StatusInfoMessage = "Başarıyla giriş yaptınız."
            });
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }


    }
}