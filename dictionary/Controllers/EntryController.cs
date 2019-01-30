﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private bool checkResult;
        public EntryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        [HttpPost("voteplus")]
        public async Task<RequestStatus> VotePlus([FromBody] Guid Id)
        {
            try
            {
                var accesToken = Request.Headers["Authorization"];
                if (accesToken.ToString() == null)
                {
                     return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Kullanıcı Girişi Yapınız"
                    });
                }
                if (Id != null)
                {
                    
                    var userdata = _unitOfWork._tokenHandler.ReadToken(accesToken) as JwtSecurityToken;

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

        [HttpPost("voteminus")]
        public async Task<RequestStatus> VoteMinus([FromBody] Guid Id)
        {
            try
            {
                var accesToken = Request.Headers["Authorization"];
                if (accesToken.ToString() == null)
                {
                    return await Task.FromResult(new RequestStatus
                    {
                        Status = false,
                        StatusInfoMessage = "Kullanıcı Girişi Yapınız"
                    });
                }

                if (Id != null)
                {
                    var userdata = _unitOfWork._tokenHandler.ReadToken(accesToken) as JwtSecurityToken;
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