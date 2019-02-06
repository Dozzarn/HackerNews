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
    public class HelperController : ControllerBase
    {
        private readonly IUnitOfWork<SearchDTO> _unitOfWork;
        public HelperController(IUnitOfWork<SearchDTO> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody]SearchDTO searchDTO)
        {
            //TODO: More Test
            try
            {
                if (!string.IsNullOrEmpty(searchDTO.Text))
                {
                    var key = $"Search:{searchDTO.Text}";
                    var isCached = await _unitOfWork._redisHandler.IsCached(key);
                    if (!isCached)
                    {
                        var data = await _unitOfWork._helperRepository.Search(searchDTO);
                        if (data.Entries != null || data.Titles != null || data.Users != null)
                        {
                            //TODO:check if statement
                            data.Status = true;
                            data.StatusInfoMessage = "Başarıyla Getirildi";
                            await _unitOfWork._redisHandler.AddToCache(key,TimeSpan.FromSeconds(30),JsonConvert.SerializeObject(data));
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

                    return await Task.FromResult(Ok(new SearchForRequestDTO {
                        Status = false,
                        StatusInfoMessage = "Boş"
                    }));
                }
            }
            catch (Exception)
            {
                return await Task.FromResult(Ok(new SearchForRequestDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                }));
                throw;
            }

        }
        

        
    }
}