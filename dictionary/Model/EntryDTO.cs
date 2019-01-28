using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class EntryDTO
    {

        public Guid EntryId { get; set; }

        public string Entry { get; set; }

        public DateTime Time { get; set; }


        public Guid UserId { get; set; }


        public Guid TitleId { get; set; }

        public int VoteMinus { get; set; }

        public int VotePlus { get; set; }
    }
}
