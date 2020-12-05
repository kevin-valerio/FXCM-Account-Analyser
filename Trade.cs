using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXCM_Analysis {
    public class Trade {
        public string Date { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public ulong Account { get; set; }
        public string Ticket { get; set; }
        public float GainOrLoss { get; set; }
        public float FinalBalance { get; set; }

    }
}
