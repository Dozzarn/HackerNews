using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController,Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EntryController : ControllerBase
    {

        private readonly IUnitOfWork<EntryDTO> _unitOfWork;
        private bool checkResult;
        bool isUpdated;
        private string allTitleData = "AllEntry:Data";
        private string getAllSql = "select* from[Entry] a inner join[User] b on a.UserId=b.Id";
        public EntryController(IUnitOfWork<EntryDTO> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Get All Entry
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall"),AllowAnonymous]
        public async Task<EntryForGelAllDTO> GetAll()
        {
            try
            {
                var isCached = await _unitOfWork._redisHandler.IsCached(allTitleData);
                if (isCached == false)
                {

                    var result = await _unitOfWork._genericRepository.GetAllAsync(getAllSql);
                    if (result != null)
                    {
                        await _unitOfWork._redisHandler.AddToCache(allTitleData, TimeSpan.FromMinutes(1), JsonConvert.SerializeObject(result));
                        return await Task.FromResult(new EntryForGelAllDTO
                        {
                            AllEntry = result,
                            Status = true,
                            StatusInfoMessage = "Başarıyla Getirildi"
                        });
                    }
                    else
                    {
                        return await Task.FromResult(new EntryForGelAllDTO
                        {
                            Status = false,
                            StatusInfoMessage = "Data Bulunamadı"
                        });
                    }
                }
                else
                {
                    var data = JsonConvert.DeserializeObject<IEnumerable<EntryDTO>>(await _unitOfWork._redisHandler.GetFromCache(allTitleData));
                    return await Task.FromResult(new EntryForGelAllDTO
                    {
                        AllEntry = data,
                        Status = true,
                        StatusInfoMessage = "Başarılı"
                    });
                }
            }
            catch (Exception)
            {
                return await Task.FromResult(new EntryForGelAllDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
                throw;
            }
        }

        /// <summary>
        /// Update Entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch("update")]
        public async Task<EntryForUpdateResultDTO> UpdateEntry([FromBody]EntryForUpdateDTO model)
        {
            try
            {
               
                if (!string.IsNullOrEmpty(model.Entry) &&  !string.IsNullOrEmpty(model.EntryId.ToString()))
                {
                    var sql = "update [Entry] set Entry =@e where EntryId=@ei";
                    var param = new { e = model.Entry, ei = model.EntryId };
                    var result = await _unitOfWork._genericRepository.UpdateAsync(sql, param);
                    if (result != false)
                    {
                        _unitOfWork.Commit();
                        UpdateAllCachedData(allTitleData, getAllSql);

                        return await Task.FromResult(new EntryForUpdateResultDTO
                        {
                            Status = true,
                            StatusInfoMessage = "Güncelleme Başarıyla Yapıldı"
                        });
                    }
                    return await Task.FromResult(new EntryForUpdateResultDTO
                    {
                        Status = false,
                        StatusInfoMessage = "Güncelleme İşlemi Yapılamadı"
                    });
                }
                return await Task.FromResult(new EntryForUpdateResultDTO
                {
                    Status = false,
                    StatusInfoMessage = "Eksikleri Doldurunuz"
                });

            }
            catch (Exception)
            {

                return await Task.FromResult(new EntryForUpdateResultDTO
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
            }
        }


        public async void UpdateAllCachedData(string key, string sql)
        {
            var result = await _unitOfWork._genericRepository.GetAllAsync(sql);
            await _unitOfWork._redisHandler.AddToCache(key, TimeSpan.FromMinutes(1), JsonConvert.SerializeObject(result));
        }
        /// <summary>
        /// Insert Entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("insert")]
        public async Task<RequestStatus> InsertEntry([FromBody] EntryForInsertDTO model)
        {
            try
            {
                var userdata = _unitOfWork.getToken(Request.Headers["Authorization"]);

                if (model.Entry != null && model.TitleId != null)
                {
                    var sql = "insert into [Entry] (Entry,TitleId,UserId) values (@e,@ti,@ui)";
                    var uid = new Guid(userdata.Claims.First(x => x.Type == "nameid").Value);
                    var param = new { e = model.Entry, ti = model.TitleId, ui = uid };
                    var result = await _unitOfWork._genericRepository.InsertAsync(sql, param);
                    if (result != false)
                    {
                        UpdateAllCachedData(allTitleData, getAllSql);
                        _unitOfWork.Commit();

                        return await Task.FromResult(new RequestStatus
                        {
                            Status = true,
                            StatusInfoMessage = "İşlem Başarılı"
                        });
                    }
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "İşlem Başarısız"
                    });
                }
                return await Task.FromResult(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = "Eksikleri Doldurunuz"
                });
            }
            catch (Exception)
            {

                return await Task.FromResult(new RequestStatus
                {
                    Status =false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
            }

        }

        /// <summary>
        /// Delete Entry
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<RequestStatus> DeleteEntry([FromBody] Guid Id)
        {

            bool isDeleted;
            try
            {
               
                if (Id != null)
                {
                    var isBinded = await _unitOfWork._titleRepository.IsBinded(Id);
                    if (isBinded != null)
                    {
                        var sql = "select * from [Entry]";
                        var list = await _unitOfWork._genericRepository.GetAllAsync(sql);
                        var data = list.OrderBy(x => x.Time).Where(x => x.TitleId == isBinded.TitleId).ToList();
                        if (data != null)
                        {
                            var second = data.ElementAt(1);
                            isBinded.EntryId = second.EntryId;
                            sql = "update [Title] set EntryId=@ei";
                            var param = new { ei = Id };
                            isUpdated = await _unitOfWork._genericRepository.UpdateAsync(sql,param);
                            _unitOfWork._entryRepository.DeleteFromVoted(Id);
                            sql = "delete from [Entry] where EntryId=@ei";
                            isDeleted = await _unitOfWork._genericRepository.DeleteAsync(sql,param);
                        }
                        else
                        {
                            return await Task.FromResult(new RequestStatus
                            {
                                Status = false,
                                StatusInfoMessage = "Kayıt Bulunamadı"
                            });
                        }
                    }
                    else
                    {
                        var sql = "delete from [Entry] where EntryId=@ei";
                        var param = new { ei = Id };
                        _unitOfWork._entryRepository.DeleteFromVoted(Id);
                        isDeleted = await _unitOfWork._genericRepository.DeleteAsync(sql,param);
                    }
                    if (isDeleted == true || isUpdated != false)
                    {
                        UpdateAllCachedData(allTitleData, getAllSql);
                        _unitOfWork.Commit();


                        return await Task.FromResult(new RequestStatus
                        {
                            Status = true,
                            StatusInfoMessage = "İşlem Başarılı"
                        });
                    }
                    else
                    {
                        return await Task.FromResult(new RequestStatus
                        {
                            Status = false,
                            StatusInfoMessage = "Kayıt Bulunamadı"
                        });
                    }

                }
                else
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Id Eksik"
                    });
                }
            }
            catch (Exception)
            {
                return await Task.FromResult(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
                throw;
            }
        }

        /// <summary>
        /// Vote Plus
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("voteplus")]
        public async Task<RequestStatus> VotePlus([FromBody] Guid Id)
        {
            try
            {
               
                if (Id != null)
                {
                    
                    var userdata = _unitOfWork.getToken(Request.Headers["Authorization"]);


                    var checkVote = await _unitOfWork._entryRepository.CheckForVote(Id);
                    if (checkVote.Status)
                    {
                        if (checkVote.StatusInfoMessage == "Artı")
                        {
                            return await Task.FromResult(new RequestStatus
                            {
                                Status = false,
                                StatusInfoMessage = "Daha Önce Artıladınız"
                            });
                        }else if(checkVote.StatusInfoMessage == "Eksi")
                        {
                            var updated =await _unitOfWork._entryRepository.UpdateToVoted(new Guid(userdata.Claims.First(x => x.Type == "nameid").Value), Id, true);
                            await _unitOfWork._entryRepository.VotePlus(Id,true);
                            checkResult = true;

                    
                        }
                        else if (checkVote.StatusInfoMessage == "Boş" )
                        {
                            await _unitOfWork._entryRepository.AddToVoted(new Guid(userdata.Claims.First(x => x.Type == "nameid").Value), Id, true);

                            await _unitOfWork._entryRepository.VotePlus(Id, false);
                            checkResult = true;

                        }
                         if (checkResult)
                            {
                            UpdateAllCachedData(allTitleData, getAllSql);
                                _unitOfWork.Commit();


                            return await Task.FromResult(new RequestStatus
                                {
                                    Status = true,
                                    StatusInfoMessage = "+1 Artılandı"
                                });
                            }
                            else
                            {
                                return await Task.FromResult(new RequestStatus
                                {
                                    Status = false,
                                    StatusInfoMessage = "başarısız"
                                });
                            }
                    }
                    else
                    {
                        return await Task.FromResult(checkVote);
                    }
                }
                else
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Id Boş"
                    });
                }
            }
            catch (Exception)
            {

                return await Task.FromResult(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
            }
        }

        /// <summary>
        /// Vote Minus
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("voteminus")]
        public async Task<RequestStatus> VoteMinus([FromBody] Guid Id)
        {
            try
            {
                    var userdata = _unitOfWork.getToken(Request.Headers["Authorization"]);

                if (Id != null)
                {
                    var checkVote = await _unitOfWork._entryRepository.CheckForVote(Id);
                    if (checkVote.Status)
                    {
                        if (checkVote.StatusInfoMessage == "Artı")
                        {
                            var updated = await _unitOfWork._entryRepository.UpdateToVoted(new Guid(userdata.Claims.First(x => x.Type == "nameid").Value), Id, false);

                            var result = await _unitOfWork._entryRepository.VoteMinus(Id, true);
                            checkResult = true;
                        }
                        else if (checkVote.StatusInfoMessage == "Eksi")
                        {

                            return await Task.FromResult(new RequestStatus
                            {
                                Status = false,
                                StatusInfoMessage = "Daha Önce Eksilediniz"
                            });

                        }
                        else if (checkVote.StatusInfoMessage == "Boş")
                        {
                            await _unitOfWork._entryRepository.AddToVoted(new Guid(userdata.Claims.First(x => x.Type == "nameid").Value), Id, false);

                            var result = await _unitOfWork._entryRepository.VoteMinus(Id, false);
                            checkResult = true;

                        }
                        if (checkResult)
                        {
                            UpdateAllCachedData(allTitleData, getAllSql);
                            _unitOfWork.Commit();


                            return await Task.FromResult(new RequestStatus
                            {
                                Status = true,
                                StatusInfoMessage = "+1 Eksilendi"
                            });
                        }
                        else
                        {
                            return await Task.FromResult(new RequestStatus
                            {
                                Status = false,
                                StatusInfoMessage = "başarısız"
                            });
                        }
                    }
                    else
                    {
                        return await Task.FromResult(checkVote);
                    }
                }
                else
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Id Boş"
                    });
                }
            }
            catch (Exception)
            {

                return await Task.FromResult(new RequestStatus
                {
                    Status = false,
                    StatusInfoMessage = "Bir Sorunla Karşılaşıldı"
                });
            }
        }
    }
}