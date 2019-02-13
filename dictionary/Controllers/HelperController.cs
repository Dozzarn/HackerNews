using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly IUnitOfWork<SearchDTO> _unitOfWork;
        public HelperController(IUnitOfWork<SearchDTO> unitOfWork, ILogger<HelperController> logger)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork._logger = logger;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody]SearchDTO searchDTO)
        {
            //TODO: More Test
            try
            {
                _unitOfWork._logger.LogInformation($"Searched for {searchDTO.Text}");
                if (!string.IsNullOrEmpty(searchDTO.Text))
                {
                    var key = $"Search:{searchDTO.Text}";
                    var isCached = await _unitOfWork._redisHandler.IsCached(key);
                    _unitOfWork._logger.LogInformation($"Is Cached {isCached}");

                    if (!isCached)
                    {
                        var data = await _unitOfWork._helperRepository.Search(searchDTO);
                        if (data.Entries != null || data.Titles != null || data.Users != null)
                        {
                            //TODO:check if statement
                            data.Status = true;
                            data.StatusInfoMessage = "Başarıyla Getirildi";
                            await _unitOfWork._redisHandler.AddToCache(key, TimeSpan.FromSeconds(30), JsonConvert.SerializeObject(data));
                            return await Task.FromResult(Ok(data));

                        }
                        else
                        {
                            data.Status = false;
                            data.StatusInfoMessage = "Veri Bulunamadı";
                            return await Task.FromResult(Ok(data));
                        }
                    }
                    else
                    {
                        var data = await _unitOfWork._redisHandler.GetFromCache(key);
                        return await Task.FromResult(Ok(JsonConvert.DeserializeObject(data)));
                    }
                }
                else
                {

                    return await Task.FromResult(Ok(new SearchForRequestDTO
                    {
                        Status = false,
                        StatusInfoMessage = "Boş"
                    }));
                }
            }
            catch (Exception exp)
            {
                _unitOfWork._logger.LogInformation($"Exception: {exp}");

                return await Task.FromResult(Ok(new SearchForRequestDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                }));
                throw;
            }

        }



        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatistic()
        {
            try
            {
                _unitOfWork._logger.LogInformation($"Get Statistics");
                var key = "PageStatistic:Statistic";
                var isCached = await _unitOfWork._redisHandler.IsCached(key);
                _unitOfWork._logger.LogInformation($"Is Cached : {isCached}");

                if (isCached)
                {
                    var cached = await _unitOfWork._redisHandler.GetFromCache(key);
                    return await Task.FromResult(Ok(JsonConvert.DeserializeObject(cached)));
                }
                else
                {
                    var data = await _unitOfWork._helperRepository.GetAllStatistic();
                    await _unitOfWork._redisHandler.AddToCache("PageStatistic:Statistic", TimeSpan.FromSeconds(30), JsonConvert.SerializeObject(data));
                    return await Task.FromResult(Ok(data));
                }
                
            }
            catch (Exception exp)
            {
                _unitOfWork._logger.LogInformation($"Exception : {exp}");
                return Ok(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = exp.ToString()

                });

                throw;
            }
        }

    }
}