using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork<UserDTO> _unitOfWork;
        public AuthController(IUnitOfWork<UserDTO> unitOfWork,ILogger<AuthController> logger)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork._logger = logger;
        }

        /// <summary>
        ///  Register 
        /// </summary>
        /// <param name="userForRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {
            try
            {
                _unitOfWork._logger.LogInformation($"Register Data: {JsonConvert.SerializeObject(userForRegisterDTO)}");

                UserForRegisterResultDTO result;

                if (await _unitOfWork._authRepository.UserExits(userForRegisterDTO.Username))
                {
                    _unitOfWork._logger.LogInformation($"Register : Kullanıcı Kayıtlı");
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
                    _unitOfWork._logger.LogInformation($"Register : Passwords Not Matched");

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
                    _unitOfWork._logger.LogInformation($"Register : Model Problem");

                    return await Task.FromResult(Ok(ModelState));
                }
                var userToCreate = new UserDTO
                {
                    Username = userForRegisterDTO.Username,
                };

                var createdUser = await _unitOfWork._authRepository.Register(userToCreate, userForRegisterDTO.Password);
                if (createdUser != null)
                {
                    _unitOfWork._logger.LogInformation($"Register : Başarıyla Kayıt Oldu");
                    _unitOfWork.Commit();
                    result = new UserForRegisterResultDTO
                    {
                        Username = createdUser.Username,
                        Status = true,
                        StatusInfoMessage = "Kullanıcı kaydı başarıyla tamamlandı"
                    };
                    return await Task.FromResult(Ok(result));
                }
                _unitOfWork._logger.LogInformation($"Register : Bir Sorunla Karşılaşıldı");
                result = new UserForRegisterResultDTO
                {

                    Username = null,
                    Status = false,
                    StatusInfoMessage = "Kayıt işlemi sırasında bir sıkıntıyla karşılaşıldı"
                };
                return await Task.FromResult(Ok(result));

            }
            catch (Exception exp)

            {
                _unitOfWork._logger.LogInformation($"Exception : {exp}");
                return await Task.FromResult(Ok(new UserForRegisterResultDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                }));
                throw;
            }


        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userForLoginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody]UserForLoginDTO userForLoginDTO)
        {
            try
            {
                var user = await _unitOfWork._authRepository.Login(userForLoginDTO.Username, userForLoginDTO.Password);
                _unitOfWork._logger.LogInformation($"Login data: {JsonConvert.SerializeObject(userForLoginDTO)}");

                if (user == null)
                {
                    _unitOfWork._logger.LogInformation($"Login Result: Unauthorized");

                    return await Task.FromResult(Unauthorized());
                }
                return await Task.FromResult(Ok(TokenHandler(user, userForLoginDTO.RememberMe)));
            }
            catch (Exception exp)
            {
                _unitOfWork._logger.LogInformation($"Exception: {exp}");
                return await Task.FromResult(Ok(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = "bir Sorunla Karşılaşıldı"
                }));
                throw;
            }
        }

        /// <summary>
        /// Get User Activity Info
        /// </summary>
        /// <returns></returns>
        [HttpGet("useractivity"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UserActivity()
        {
            try
            {

                var userdata = _unitOfWork.getToken(Request.Headers["Authorization"]);

                var userId = new Guid(userdata.Claims.First(x => x.Type == "nameid").Value);
                _unitOfWork._logger.LogInformation($"User Activity For : {userId}");

                var data = await _unitOfWork._authRepository.GetTotals(userId);
                return await Task.FromResult(Ok(data));
            }
            catch (Exception exp)
            {
                _unitOfWork._logger.LogInformation($"Exception: {exp}");
                return await Task.FromResult(Ok(
                    new TotalActivityDTO
                    {
                        Status = false,
                        StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                    }));
            }
        }

        private UserForLoginResultDTO TokenHandler(UserDTO user, bool rememberMe)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_unitOfWork._configuration.GetSection("AppSettings:Token").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                     new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                     new Claim(ClaimTypes.Name,user.Username)


                }),
                Expires = rememberMe ? DateTime.Now.AddDays(365) : DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return new UserForLoginResultDTO
            {
                Token = tokenString,
                Status = true,
                StatusInfoMessage = "Başarıyla giriş yaptınız."
            };
        }



    }
}