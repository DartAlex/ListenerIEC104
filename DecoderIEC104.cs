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
        // Format control field
        private static string GetFormatControlFields(byte[] bytes)
        {
            string data = null;

            BitArray bitsField1 = GetBitsOfByte(bytes[2]);
            BitArray bitsField2 = GetBitsOfByte(bytes[3]);
            BitArray bitsField3 = GetBitsOfByte(bytes[4]);
            BitArray bitsField4 = GetBitsOfByte(bytes[5]);

            if (bitsField1[0] == false)
            {
                data = "Format I";
            }

            if (bitsField1[0] == true && bitsField1[1] == false)
            {
                data = "Format S";
            }

            if (bitsField1[0] == true && bitsField1[1] == true)
            {                
                bool startdtAct = bitsField1[2];
                bool startdtCon = bitsField1[3];

                bool stopDtAct = bitsField1[4];
                bool stopDtCon = bitsField1[5];

                bool testfrAct = bitsField1[6];
                bool testfrCon = bitsField1[7];

                if (startdtAct == true)
                {
                    data = "Format U" + Environment.NewLine + "STARTDT act - Старт передачи данных. Активация.";
                }

                if (startdtCon == true)
                {
                    data = "Format U" + Environment.NewLine + "STARTDT con - Старт передачи данных. Подтверждение.";
                }

                if (stopDtAct == true)
                {
                    data = "Format U" + Environment.NewLine + "STOPDT act - Прекращение передачи данных. Активация.";
                }

                if (stopDtCon == true)
                {
                    data = "Format U" + Environment.NewLine + "STOPDT con - Прекращение передачи данных. Подтверждение.";
                }

                if (testfrAct == true)
                {
                    data = "Format U" + Environment.NewLine + "TESTDT act - Тестовый блок. Активация.";
                }

                if (testfrCon == true)
                {
                    data = "Format U" + Environment.NewLine + "TESTDT con - Тестовый блок. Подтверждение.";
                }               
            }
            return (data);
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
            string data = null;

            for (int i = 0; i <= PacketLenght(bytes) + 1; i++)
            {
                data = data + Convert.ToString(bytes[i], 2).PadLeft(8, '0') + " ";
            }
            return (data);
        }

        // Analis packet 
        //
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

        // Leght ASDU
        private static int PacketLenght(byte[] bytes)
        {
            // Second byte - packet length
            return (Convert.ToInt32(bytes[1]));
        }

        // Format control field
        private static int FormatControlFields(byte[] bytes)
        {
            int result = 0;

            BitArray bitsField1 = GetBitsOfByte(bytes[2]);
            BitArray bitsField2 = GetBitsOfByte(bytes[3]);
            BitArray bitsField3 = GetBitsOfByte(bytes[4]);
            BitArray bitsField4 = GetBitsOfByte(bytes[5]);

            // Format I
            if (bitsField1[0] == false)
            {
                result = 1;
            }
            // Format S
            if (bitsField1[0] == true && bitsField1[1] == false)
            {
                result = 2;
            }
            // Format U
            if (bitsField1[0] == true && bitsField1[1] == true)
            {
                result = 3;

                bool startDtAct = bitsField1[2];
                bool startDtCon = bitsField1[3];

                bool stopDtAct = bitsField1[4];
                bool stopDtCon = bitsField1[5];

                bool testDtAct = bitsField1[6];
                bool testDtCon = bitsField1[7];

                // STARTDT act - Старт передачи данных. Активация.
                if (startDtAct == true)
                {

                }
                // STARTDT con - Старт передачи данных. Подтверждение.
                if (startDtCon == true)
                {

                }
                // STOPDT act - Прекращение передачи данных. Активация.
                if (stopDtAct == true)
                {

                }
                // STOPDT con - Прекращение передачи данных. Подтверждение.
                if (stopDtCon == true)
                {

                }
                // TESTDT act - Тестовый блок. Активация.
                if (testDtAct == true)
                {

                }
                // "TESTDT con - Тестовый блок. Подтверждение."
                if (testDtCon == true)
                {

                }
            }
            return (result);
        }
        
        // Reading APCI
        public static void ReadAPCI(byte[] bytes)
        {
            int leghtASDU;
            
            // Check header
            if (HeaderByte(bytes) != true)
            {
                FormMain.EventSend.AppendTextBox("Protocol is not IEC104");
                return;                
            }
            else
            {
                FormMain.EventSend.AppendTextBox("Protocol - IEC104");
            }

            // PacketLenght
            leghtASDU = PacketLenght(bytes);
            FormMain.EventSend.AppendTextBox("Packet lenght = " + leghtASDU.ToString());

            // Format control field
            switch (FormatControlFields(bytes))
            {
                case 1:
                    FormMain.EventSend.AppendTextBox("Format I");
                    break;
                case 2:
                    FormMain.EventSend.AppendTextBox("Format S");
                    break;
                case 3:
                    FormMain.EventSend.AppendTextBox("Format U");
                    break;
                default:
                    FormMain.EventSend.AppendTextBox("Format unknown");
                    break;
            }           
        }
    }
}
