using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public EntryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("voteplus")]
        public async Task<RequestStatus> VotePlus([FromBody] Guid Id)
        {
            try
            {
                if (Id != null)
                {
                    var result = await _unitOfWork._entryRepository.VotePlus(Id);
                    if (result.Status)
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
                if (Id != null)
                {
                    var result = await _unitOfWork._entryRepository.VoteMinus(Id);
                    if (result.Status)
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