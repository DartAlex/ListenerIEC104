using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ListenerIEC104
{
    class DecoderIEC104
    {       
        // Check header packet      
        private static bool HeaderByte(byte[] bytes)
        {
            bool result = false;
            if (Convert.ToInt32(bytes[0]) == 104)
            {
                result = true;
            }
            return (result);
        }

        // Get leght ASDU
        private static int PacketLenght(byte[] bytes)
        {
            // Second byte - packet length
            return (Convert.ToInt32(bytes[1]));
        }

        private static string GetFormatControlFields(byte[] bytes)
        {
            /*BitArray bitsField1 = new BitArray(bytes[2]);
            BitArray bitsField2 = new BitArray(bytes[3]);
            BitArray bitsField3 = new BitArray(bytes[4]);
            BitArray bitsField4 = new BitArray(bytes[5]);

            string data = null;
            foreach (bool b in bitsField1)
            {
                data = data + b.ToString();
            }*/

            return (Environment.NewLine);
        }

        private static string GetStringOfByte(byte valueByte)
        {
            string binByte = Convert.ToString(valueByte, 2).PadLeft(8, '0');
            return (binByte);
        }

        private static int GetIntOfBytes(byte valueByte)
        {
            Int32 decByte = Convert.ToInt32(valueByte);
            return (decByte);
        }

        /*private static int[] GetBitsOfByte(byte valueByte)
        {
         
        }*/

        // To String
        public static string IECToString(byte[] bytes)
        {
            string data = "Received: " + Environment.NewLine;

            for (int i = 0; i <= PacketLenght(bytes) + 1; i++)
            {
                data = data + Convert.ToString(bytes[i], 2).PadLeft(8, '0') + " ";
            }

            data = data + Environment.NewLine;
            data = data + "Start 68H:" + GetStringOfByte(bytes[0]) + " - " + GetIntOfBytes(bytes[0]).ToString() + Environment.NewLine;
            data = data + "Lenght APDU:" + GetStringOfByte(bytes[1]) + " - " + GetIntOfBytes(bytes[1]).ToString() + Environment.NewLine;
            data = data + "Сontrol field byte 1:" + GetStringOfByte(bytes[2]).ToString() + Environment.NewLine;
            data = data + "Сontrol field byte 2:" + GetStringOfByte(bytes[3]) + Environment.NewLine;
            data = data + "Сontrol field byte 3:" + GetStringOfByte(bytes[4]) + Environment.NewLine;
            data = data + "Сontrol field byte 4:" + GetStringOfByte(bytes[5]) + Environment.NewLine;            

            return (data);
        }
    }
}
