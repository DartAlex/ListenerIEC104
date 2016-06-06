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
            BitArray bitsField1 = GetBitsOfByte(bytes[2]);
            BitArray bitsField2 = GetBitsOfByte(bytes[3]);
            BitArray bitsField3 = GetBitsOfByte(bytes[4]);
            BitArray bitsField4 = GetBitsOfByte(bytes[5]);

            // Format I
            // | 8 | 7 | 6 | 5 | 4 | 3 | 2 | 1 |
            // | Tx index number N(S)   LSB| 0 | - byte 1
            // |MSB   TX index number N(S)     | - byte 2
            // | Rx index number N(R)   LSB| 0 | - byte 3
            // |MSB   TX index number N(R)     | - byte 4
            //
            // MSB - Senior bit
            // LSB - Least bit

            if (bitsField1[0] == false)
            {
                bool[] txIndexBool = new bool[15] 
                {
                    bitsField1[1],
                    bitsField1[2],
                    bitsField1[3],
                    bitsField1[4],
                    bitsField1[5],
                    bitsField1[6],
                    bitsField1[7],
                    bitsField2[0],
                    bitsField2[1],
                    bitsField2[2],
                    bitsField2[3],
                    bitsField2[4],
                    bitsField2[5],
                    bitsField2[6],
                    bitsField2[7]
                };

                BitArray txIndexBitArray = new BitArray(txIndexBool);
                var result = new int[1];
                txIndexBitArray.CopyTo(result, 0);
                //txIndexBitArray.CopyTo(result, 0)

                //BitArray TxIndexNum = new BitArray(new BitArray { bitsField1[1] }); 

                return ("Format I " + result.ToString());
            }

            if (bitsField1[0] == true && bitsField1[1] == false)
            {
                return ("Format S");
            }

            if (bitsField1[0] == true && bitsField1[1] == true)
            {
                return ("Format U");
            }


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

        private static BitArray GetBitsOfByte(byte valueByte)
        {
            BitArray bits = new BitArray(new byte[] { valueByte });
            return (bits);                      
        }

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

            data = data + GetFormatControlFields(bytes);

            data = data + Environment.NewLine;

            return (data);
        }
    }
}
