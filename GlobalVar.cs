using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerIEC104
{
    class GlobalVar
    {
        public static bool threadingRun { get; set; }
        public static Int32 portListen { get; set; }
        public static string ipSender { get; set; }
        public static Int32 portSender { get; set; }        
    }
}
