using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TitleForGetAllDTO:RequestStatus
    {
        [JsonProperty("t")]
        public IEnumerable<TitleDTO> Titles  { get; set; }


    }
}
