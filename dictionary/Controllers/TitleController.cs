using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly IUnitOfWork<TitleDTO> _unitOfWork;
        private string allTitleData = "AllTitle:Data";
        public TitleController(IUnitOfWork<TitleDTO> unitOfWork)
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
                var sql = "select a.*,b.Entry from [Title] a inner join [Entry] b on a.EntryId=b.EntryId";
                var data = await _unitOfWork._genericRepository.GetAllAsync(sql);
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

        /// <summary>
        /// Get Title
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost("get")]
        public async Task<TitleForGetDTO> Get([FromBody]Guid guid)
        {
            var key = $"OnlyTitle:{guid.ToString()}";
            var isCached = await _unitOfWork._redisHandler.IsCached(key);
            if (isCached == false)
            {
                var sql = "select * from [Title] where TitleId=@id";
                var param = new { id = guid };
                var title = await _unitOfWork._genericRepository.GetByIdAsync(sql, param);
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