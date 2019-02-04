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
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork<UserDTO> _unitOfWork;
        public AuthController(IUnitOfWork<UserDTO> unitOfWork)
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
            var userToCreate = new UserDTO
            {
                Username = userForRegisterDTO.Username,
            };

            var createdUser = await _unitOfWork._authRepository.Register(userToCreate, userForRegisterDTO.Password);
            if (createdUser != null)
            {
                _unitOfWork.Commit();
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
            return await Task.FromResult(Ok(TokenHandler(user, userForLoginDTO.RememberMe)));
        }

        /// <summary>
        /// Get User Activity Info
        /// </summary>
        /// <returns></returns>
        [HttpGet("useractivity")]
        public async Task<TotalActivityDTO> UserActivity()
        {
            try
            {
                if (!_unitOfWork.Check(Request.Headers["Authorization"]))
                {
                    var userId = new Guid(_unitOfWork.userdata.Claims.First(x => x.Type == "nameid").Value);
                    var key = $"User:Activity:{userId}";
                    var isCached = await _unitOfWork._redisHandler.IsCached(key);
                    if (isCached == false)
                    {
                        var data = await _unitOfWork._authRepository.GetTotals(userId);
                        await _unitOfWork._redisHandler.AddToCache(key, TimeSpan.FromMinutes(1), JsonConvert.SerializeObject(data));
                        return await Task.FromResult(data);

                    }
                    else
                    {
                        var data = JsonConvert.DeserializeObject<TotalActivityDTO>(await _unitOfWork._redisHandler.GetFromCache(key));
                        return await Task.FromResult(data);
                    }
                }
                else
                {
                    return await Task.FromResult(new TotalActivityDTO
                    {
                        Status = false,
                        StatusInfoMessage = "Kullanıcı Girişi Yapınız"
                    });
                }

            }
            catch (Exception)
            {
                return await Task.FromResult(new TotalActivityDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
                throw;
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