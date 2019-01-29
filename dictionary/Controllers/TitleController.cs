using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private string allTitleData = "AllTitle:Data";
        public TitleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get All Title
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<TitleForGetAllDTO> GetAll()
        {

            var isCached = await _unitOfWork._redisHandler.IsCached(allTitleData);
            if (isCached == false)
            {
                var data = await _unitOfWork._titleRepository.GetAll();
                if (data != null)
                {
                    await _unitOfWork._redisHandler.AddToCache(allTitleData, TimeSpan.FromMinutes(10), JsonConvert.SerializeObject(data));
                    return await Task.FromResult(new TitleForGetAllDTO
                    {
                        Titles = data,
                        Status = true,
                        StatusInfoMessage = "Başarılı"
                    });
                }
                else
                {
                    return await Task.FromResult(new TitleForGetAllDTO
                    {
                        Titles = null,
                        Status = false,
                        StatusInfoMessage = "başarısız"
                    });
                }

            }
            else
            {
                var data = JsonConvert.DeserializeObject<IEnumerable<TitleDTO>>(await _unitOfWork._redisHandler.GetFromCache(allTitleData));
                return await Task.FromResult(new TitleForGetAllDTO
                {
                    Titles = data,
                    Status = true,
                    StatusInfoMessage = "Başarılı"
                });

            }



        }

        [HttpPost("get")]
        public async Task<TitleForGetDTO> Get([FromBody]Guid guid)
        {
            var key = $"OnlyTitle:{guid.ToString()}";
            var isCached = await _unitOfWork._redisHandler.IsCached(key);
            if (isCached == false)
            {
                var title = await _unitOfWork._titleRepository.GetById(guid);
                var entries = await _unitOfWork._entryRepository.GetAllEntryForTitle(guid);

                if (title != null)
                {
                    var result = new TitleForGetDTO
                    {
                        Title = title,
                        Entries = entries,
                        Status = true,
                        StatusInfoMessage = "Başarılı"
                    };
                    await _unitOfWork._redisHandler.AddToCache(key, TimeSpan.FromMinutes(10), JsonConvert.SerializeObject(result));

                    return await Task.FromResult(result);
                }
                return await Task.FromResult(new TitleForGetDTO
                {
                    Title = title,
                    Status = false,
                    StatusInfoMessage = "Başarısız"
                });
            }
            else
            {
                var result = JsonConvert.DeserializeObject<TitleForGetDTO>(await _unitOfWork._redisHandler.GetFromCache(key));
                return await Task.FromResult(result);
            }

        }
    }
}