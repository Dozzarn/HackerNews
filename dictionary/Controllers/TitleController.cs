using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController,]
    public class TitleController : ControllerBase
    {
        private readonly IUnitOfWork<TitleDTO> _unitOfWork;
        private string allTitleData = "AllTitle:Data";
        private string getAllSql = "select a.*,b.Entry from [Title] a inner join [Entry] b on a.EntryId=b.EntryId";
        public TitleController(IUnitOfWork<TitleDTO> unitOfWork,ILogger<TitleController> logger)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork._logger = logger;
        }

      
        /// <summary>
        /// Add Title 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpPost("insert"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Insert([FromBody] TitleForInsertDTO title)
        {
            try
            {
                _unitOfWork._logger.LogInformation($"Title Insert:  {JsonConvert.SerializeObject(title)}");
                var userdata = _unitOfWork.getToken(Request.Headers["Authorization"]);

                var sql = "select * from [Title]";
                var p = new { t = title.Title };
                var list = await _unitOfWork._genericRepository.GetAllAsync(sql);
                var isExists = list.Where(x => x.Title == title.Title).ToList();

                if (isExists.Count !=0)
                {
                    return await Task.FromResult(Ok(new TitleForInsertResultDTO
                    {
                        Status = false,
                        StatusInfoMessage = "Title Daha Önce Açılmış Kardeş"
                    }));
                }

                var tid = Guid.NewGuid();
                var uid = new Guid(userdata.Claims.First(x => x.Type == "nameid").Value);
                var eid = Guid.NewGuid();
                _unitOfWork._logger.LogInformation($"Title Insert: User {uid}");

                sql = "insert into [Entry] (EntryId,Entry,UserId,TitleId) values(@ei,@e,@ui,@ti)";

                var param2 = new { ei = eid, e = title.Entry, ui = uid, ti = tid };
                var result2 = await _unitOfWork._genericRepository.InsertAsync(sql, param2);

                sql = "insert into [Title] (TitleId,Title,UserId,EntryId,Category)values(@ti,@t,@ui,@ei,@cat)";

                var param = new { ti = tid, t = title.Title, ui = uid, ei = eid, cat = title.Category };
                var result = await _unitOfWork._genericRepository.InsertAsync(sql,param);
               
                if (result && result2)
                {
                    UpdateAllCachedData(allTitleData, getAllSql);
                    _unitOfWork.Commit();
                    return await Task.FromResult(Ok(new TitleForInsertResultDTO
                    {
                        Status = true,
                        StatusInfoMessage = "Title Başarıyla Açıldı"
                    }));
                }
                else
                {
                    return await Task.FromResult(Ok(new TitleForInsertResultDTO
                    {
                        Status = false,
                        StatusInfoMessage = "Bır Sıkıntı oldi"

                    }));
                }
            }
            catch (Exception exp)
            {
                _unitOfWork._logger.LogInformation($"Exception :  {exp}");

                return await Task.FromResult(Ok(new TitleForInsertResultDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                }));

            }
        }

        public async void UpdateAllCachedData(string key, string sql)
        {
            var result = await _unitOfWork._genericRepository.GetAllAsync(sql);
            await _unitOfWork._redisHandler.AddToCache(key, TimeSpan.FromMinutes(1), JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Get All Title
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                var isCached = await _unitOfWork._redisHandler.IsCached(allTitleData);
                if (isCached == false)
                {
                    var data = await _unitOfWork._genericRepository.GetAllAsync(getAllSql);
                    if (data != null)
                    {
                        await _unitOfWork._redisHandler.AddToCache(allTitleData, TimeSpan.FromMinutes(1), JsonConvert.SerializeObject(data));
                        return await Task.FromResult(Ok(new TitleForGetAllDTO
                        {
                            Titles = data,
                            Status = true,
                            StatusInfoMessage = "Başarılı"
                        }));
                    }
                    else
                    {
                        return await Task.FromResult(Ok(new TitleForGetAllDTO
                        {
                            Status = false,
                            StatusInfoMessage = "başarısız"
                        }));
                    }

                }
                else
                {
                    var data = JsonConvert.DeserializeObject<IEnumerable<TitleDTO>>(await _unitOfWork._redisHandler.GetFromCache(allTitleData));
                    return await Task.FromResult(Ok(new TitleForGetAllDTO
                    {
                        Titles = data,
                        Status = true,
                        StatusInfoMessage = "Başarılı"
                    }));

                }
            }
            catch (Exception exp )
            {
                _unitOfWork._logger.LogInformation($"Exception :  {exp}");

                return await Task.FromResult(Ok(new TitleForGetAllDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                }));

                throw;
            }



        }

        /// <summary>
        /// Get Title
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost("get")]
        public async Task<IActionResult> Get([FromBody]Guid guid)
        {
            try
            {
                _unitOfWork._logger.LogInformation($"Title Get  :  {guid.ToString()}");

                var key = $"OnlyTitle:{guid.ToString()}";
                var isCached = await _unitOfWork._redisHandler.IsCached(key);
                _unitOfWork._logger.LogInformation($"Title Get  : Is cached {isCached}");

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
                        await _unitOfWork._redisHandler.AddToCache(key, TimeSpan.FromMinutes(1), JsonConvert.SerializeObject(result));

                        return await Task.FromResult(Ok(result));
                    }
                    return await Task.FromResult(Ok(new TitleForGetDTO
                    {
                        Title = title,
                        Status = false,
                        StatusInfoMessage = "Başarısız"
                    }));
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<TitleForGetDTO>(await _unitOfWork._redisHandler.GetFromCache(key));
                    return await Task.FromResult(Ok(result));
                }

            }
            catch (Exception exp)
            {
                _unitOfWork._logger.LogInformation($"Exception :  {exp}");

                return await Task.FromResult(Ok(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorun Oldu"
                }));
                throw;
            }
        }


    }
}