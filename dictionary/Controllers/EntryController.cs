﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using dictionary.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {

        private readonly IUnitOfWork<EntryDTO> _unitOfWork;
        private bool checkResult;
        private JwtSecurityToken userdata;
        bool isUpdated;

        public EntryController(IUnitOfWork<EntryDTO> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        
        //TODO:CHANGE NECESSARY SQL QUERY TO GETALL().WHERE BLA BLA BLA
        private bool Check()
        {
            var accesToken = Request.Headers["Authorization"];
            if (accesToken.ToString() == null)
            {
                return false;
            }
            userdata = _unitOfWork._tokenHandler.ReadToken(accesToken) as JwtSecurityToken;
            return true;
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
                if (!Check())
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Kullanıcı Girişi Yapınız"
                    });
                }
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
                if (!Check())
                {
                    return new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Kullanıcı Girişi Yapınız"
                    };
                }
                if (Id != null)
                {
                    
                    

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
                            //await _unitOfWork._entryRepository.AddToVoted(new Guid(userdata.Claims.First(x => x.Type == "nameid").Value), Id, true);
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
                if (!Check())
                {
                    return new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Kullanıcı Girişi Yapınız"
                    };
                }

                if (Id != null)
                {
                    var checkVote = await _unitOfWork._entryRepository.CheckForVote(Id);
                    if (checkVote.Status)
                    {
                        if (checkVote.StatusInfoMessage == "Artı")
                        {
                            //await _unitOfWork._entryRepository.AddToVoted(new Guid(userdata.Claims.First(x => x.Type == "nameid").Value), Id, false);
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