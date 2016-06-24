using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ListenerIEC104
{
    class EncoderIEC104
    {        
        // Format U
        public static byte[] FormatUStartAct()
        {            
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 00000111
            byte b3 = Convert.ToByte(7);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }       
        public static byte[] FormatUStartCon()
        {           
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 00001011
            byte b3 = Convert.ToByte(11);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }

        public static byte[] FormatUStopAct()
        {          
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 00010011
            byte b3 = Convert.ToByte(19);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }

        public static byte[] FormatUStopCon()
        {            
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 00100011
            byte b3 = Convert.ToByte(35);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }

        public static byte[] FormatUTestAct()
        {            
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 01000011
            byte b3 = Convert.ToByte(67);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }

        public static byte[] FormatUTestCon()
        {
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 10000011
            byte b3 = Convert.ToByte(131);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }

        public static byte[] Default()
        {
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            // 00000000
            byte b3 = Convert.ToByte(0);
            byte b4 = Convert.ToByte(0);
            byte b5 = Convert.ToByte(0);
            byte b6 = Convert.ToByte(0);

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }

        // Format I
        public static byte[] FormatI(int sequenceNumber)
        {
            byte b1 = Convert.ToByte(104);
            byte b2 = Convert.ToByte(4);
            byte b3 = Convert.ToByte(0);
            byte b4 = Convert.ToByte(0);

            /*BitArray bitSequenceNumberNS = new BitArray(new int[] { sequenceNumber });

            BitArray bitArrayNS = new BitArray(16, false);

            bitArrayNS[0] = false;
            bitArrayNS[1] = bitSequenceNumberNS[0];
            bitArrayNS[2] = bitSequenceNumberNS[1];
            bitArrayNS[3] = bitSequenceNumberNS[2];
            bitArrayNS[4] = bitSequenceNumberNS[3];
            bitArrayNS[5] = bitSequenceNumberNS[4];
            bitArrayNS[6] = bitSequenceNumberNS[5];
            bitArrayNS[7] = bitSequenceNumberNS[6];
            bitArrayNS[8] = bitSequenceNumberNS[7];
            bitArrayNS[9] = bitSequenceNumberNS[8];
            bitArrayNS[10] = bitSequenceNumberNS[9];
            bitArrayNS[11] = bitSequenceNumberNS[10];
            bitArrayNS[12] = bitSequenceNumberNS[11];
            bitArrayNS[13] = bitSequenceNumberNS[12];
            bitArrayNS[14] = bitSequenceNumberNS[13];
            bitArrayNS[15] = bitSequenceNumberNS[14];

            byte[] b3b4 = new byte[2];
            bitArrayNS.CopyTo(b3b4, 0);

            byte b3 = b3b4[0];
            byte b4 = b3b4[1];*/

            BitArray bitSequenceNumberNR = new BitArray(new int[] { sequenceNumber + 1 });

            BitArray bitArrayNR = new BitArray(16, false);

            bitArrayNR[0] = false;
            bitArrayNR[1] = bitSequenceNumberNR[0];
            bitArrayNR[2] = bitSequenceNumberNR[1];
            bitArrayNR[3] = bitSequenceNumberNR[2];
            bitArrayNR[4] = bitSequenceNumberNR[3];
            bitArrayNR[5] = bitSequenceNumberNR[4];
            bitArrayNR[6] = bitSequenceNumberNR[5];
            bitArrayNR[7] = bitSequenceNumberNR[6];
            bitArrayNR[8] = bitSequenceNumberNR[7];
            bitArrayNR[9] = bitSequenceNumberNR[8];
            bitArrayNR[10] = bitSequenceNumberNR[9];
            bitArrayNR[11] = bitSequenceNumberNR[10];
            bitArrayNR[12] = bitSequenceNumberNR[11];
            bitArrayNR[13] = bitSequenceNumberNR[12];
            bitArrayNR[14] = bitSequenceNumberNR[13];
            bitArrayNR[15] = bitSequenceNumberNR[14];

            byte[] b5b6 = new byte[2];
            bitArrayNR.CopyTo(b5b6, 0);

            byte b5 = b5b6[0];
            byte b6 = b5b6[1];

            byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };
            return (answer);
        }


        // CreateHeaderIEC104
        private byte HeaderIEC104()
        {
            byte b = Convert.ToByte(104);
            return(b);
        }
    }
}
