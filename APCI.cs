using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerIEC104
{
    class APCI
    {
        public bool headerByte { get; set; }
        public int lenghtAPDU { get; set; }
        public bool formatI { get; set; }
        public int nS { get; set; }
        public int nR { get; set; }
        public bool formatS { get; set; }
        public bool formatU { get; set; }
        public bool startDT { get; set; }
        public bool stopDT { get; set; }
        public bool testFR { get; set; }
        public bool act { get; set; }
        public bool con { get; set; }

    }
}
