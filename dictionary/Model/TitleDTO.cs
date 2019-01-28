using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Model
{
    public class TitleDTO : RequestStatus
    {
        public Guid TitleId { get; set; }

        public string Title { get; set; }

        public Guid UserId { get; set; }

        public DateTime Time { get; set; }

        public Guid Category { get; set; }

        public int VoteMinus { get; set; }

        public int VotePlus { get; set; }
    }
}
