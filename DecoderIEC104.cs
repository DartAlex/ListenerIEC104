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

            for (int i = 0; i <= PacketLenght(bytes) - 1; i++)
            {
                data = data + Convert.ToString(bytes[i], 2).PadLeft(8, '0') + " ";
            }
            return (data);
        }

        // Leght ASDU
        public static int PacketLenght(byte[] bytes)
        {
            // Second byte - packet length
            return (Convert.ToInt32(bytes[1]));
        }

        public static void Read(byte[] bytes)
        {
            byte[] bytesAPCI = new byte[] 
            { 
                bytes[0],
                bytes[1],
                bytes[2],
                bytes[3],
                bytes[4],
                bytes[5]
            };

            APCI apci = ReadAPCI(bytesAPCI);
          
            if (apci.headerByte == true)
            {
                FormMain.EventSend.AppendTextBox("Protocol is IEC104");
                FormMain.EventSend.AppendTextBox("Lenght APDU: " + apci.lenghtAPDU + 2);
                
            }
        }

        private static APCI ReadAPCI(byte[] bytes)
        {
            APCI apci = new APCI() 
            { 
                headerByte = false, 
                lenghtAPDU = 0, 
                formatI = false, 
                formatS = false, 
                nR = 0, 
                nS = 0, 
                formatU = false, 
                act = false, 
                con = false, 
                startDT = false, 
                stopDT = false, 
                testFR = false 
            };

            if (Convert.ToInt32(bytes[0]) == 104)
            {
                apci.headerByte = true;
                apci.lenghtAPDU = Convert.ToInt32(bytes[1]);

                // FormatControlFields
                BitArray bitsField1 = GetBitsOfByte(bytes[2]);
                BitArray bitsField2 = GetBitsOfByte(bytes[3]);
                BitArray bitsField3 = GetBitsOfByte(bytes[4]);
                BitArray bitsField4 = GetBitsOfByte(bytes[5]);

                if (bitsField1[0] == false) // Format I
                {
                    apci.formatI = true;
                    //FormMain.EventSend.AppendTextBox("Format I");

                    // Transmitted sequence number N(S)
                    BitArray bitNS = new BitArray(15, false);

                    bitNS[0] = bitsField1[1];
                    bitNS[1] = bitsField1[2];
                    bitNS[2] = bitsField1[3];
                    bitNS[3] = bitsField1[4];
                    bitNS[4] = bitsField1[5];
                    bitNS[5] = bitsField1[6];
                    bitNS[6] = bitsField1[7];
                    bitNS[7] = bitsField2[0];
                    bitNS[8] = bitsField2[1];
                    bitNS[9] = bitsField2[2];
                    bitNS[10] = bitsField2[3];
                    bitNS[11] = bitsField2[4];
                    bitNS[12] = bitsField2[5];
                    bitNS[13] = bitsField2[6];
                    bitNS[14] = bitsField2[7];

                    int[] intNS = new int[1];
                    bitNS.CopyTo(intNS, 0);

                    apci.nS = intNS[0];
                    //FormMain.EventSend.AppendTextBox("N(S): " + intNS[0].ToString());

                    // Received sequence number N(R)
                    BitArray bitNR = new BitArray(15, false);

                    bitNR[0] = bitsField3[1];
                    bitNR[1] = bitsField3[2];
                    bitNR[2] = bitsField3[3];
                    bitNR[3] = bitsField3[4];
                    bitNR[4] = bitsField3[5];
                    bitNR[5] = bitsField3[6];
                    bitNR[6] = bitsField3[7];
                    bitNR[7] = bitsField4[0];
                    bitNR[8] = bitsField4[1];
                    bitNR[9] = bitsField4[2];
                    bitNR[10] = bitsField4[3];
                    bitNR[11] = bitsField4[4];
                    bitNR[12] = bitsField4[5];
                    bitNR[13] = bitsField4[6];
                    bitNR[14] = bitsField4[7];

                    int[] intNR = new int[1];
                    bitNR.CopyTo(intNR, 0);
                    apci.nR = intNR[0];
                    //FormMain.EventSend.AppendTextBox("N(R): " + intNR[0].ToString());

                }
                // Format S
                if (bitsField1[0] == true && bitsField1[1] == false)
                {
                    //FormMain.EventSend.AppendTextBox("Format S");
                    apci.formatS = true;

                    // Received sequence number N(R)
                    BitArray bitNR = new BitArray(15, false);

                    bitNR[0] = bitsField3[1];
                    bitNR[1] = bitsField3[2];
                    bitNR[2] = bitsField3[3];
                    bitNR[3] = bitsField3[4];
                    bitNR[4] = bitsField3[5];
                    bitNR[5] = bitsField3[6];
                    bitNR[6] = bitsField3[7];
                    bitNR[7] = bitsField4[0];
                    bitNR[8] = bitsField4[1];
                    bitNR[9] = bitsField4[2];
                    bitNR[10] = bitsField4[3];
                    bitNR[11] = bitsField4[4];
                    bitNR[12] = bitsField4[5];
                    bitNR[13] = bitsField4[6];
                    bitNR[14] = bitsField4[7];

                    int[] intNR = new int[1];
                    bitNR.CopyTo(intNR, 0);
                    apci.nR = intNR[0];
                    //FormMain.EventSend.AppendTextBox("N(R): " + intNR[0].ToString());
                }
                // Format U
                if (bitsField1[0] == true && bitsField1[1] == true)
                {
                    apci.formatU = true;

                    bool startDtAct = bitsField1[2];
                    bool startDtCon = bitsField1[3];

                    bool stopDtAct = bitsField1[4];
                    bool stopDtCon = bitsField1[5];

                    bool testFrAct = bitsField1[6];
                    bool testFrCon = bitsField1[7];

                    // STARTDT act - Старт передачи данных. Активация.
                    if (startDtAct == true)
                    {
                        apci.startDT = true;
                        apci.act = true;
                        //FormMain.EventSend.AppendTextBox("Format U STARTDT act - Старт передачи данных. Активация.");
                        //answer = EncoderIEC104.FormatUStartCon();
                    }
                    // STARTDT con - Старт передачи данных. Подтверждение.
                    if (startDtCon == true)
                    {
                        apci.startDT = true;
                        apci.con = true;
                        //FormMain.EventSend.AppendTextBox("Format U STARTDT con - Старт передачи данных. Подтверждение.");
                    }
                    // STOPDT act - Прекращение передачи данных. Активация.
                    if (stopDtAct == true)
                    {
                        apci.stopDT = true;
                        apci.act = true;
                        //FormMain.EventSend.AppendTextBox("Format U STOPDT act - Прекращение передачи данных. Активация.");
                        //answer = EncoderIEC104.FormatUStopCon();
                    }
                    // STOPDT con - Прекращение передачи данных. Подтверждение.
                    if (stopDtCon == true)
                    {
                        apci.stopDT = true;
                        apci.con = true;
                        //FormMain.EventSend.AppendTextBox("Format U STOPDT con - Прекращение передачи данных. Подтверждение.");
                    }
                    // TESTFR act - Тестовый блок. Активация.
                    if (testFrAct == true)
                    {
                        apci.testFR = true;
                        apci.act = true;
                        //FormMain.EventSend.AppendTextBox("Format U TESTFR act - Тестовый блок. Активация.");
                        //answer = EncoderIEC104.FormatUTestCon();
                    }
                    // "TESTFR con - Тестовый блок. Подтверждение."
                    if (testFrCon == true)
                    {
                        apci.testFR = true;
                        apci.con = true;
                        //FormMain.EventSend.AppendTextBox("Format U TESTFR con - Тестовый блок. Подтверждение.");
                    }
                }
            }
            else
            {
                apci.headerByte = false;
            }

            return (apci);
        }

        // ASDU
        private static void ReadASDU(byte[] bytes)
        {
            int lenght = PacketLenght(bytes);

            // Индетификатор типов
            if (lenght >= 5)
            {
                int intType = GetIntOfBytes(bytes[4]);
                FormMain.EventSend.AppendTextBox("Индетификатор типов: " + ASDUinfo.TypeIdentifier(intType));
            }
            // Классификатор переменной структуры
            if (lenght >=6)
            {
                BitArray bits = GetBitsOfByte(bytes[5]);

                BitArray bitsN = new BitArray(7, false);
                bitsN[0] = bits[0];
                bitsN[1] = bits[1];
                bitsN[2] = bits[2];
                bitsN[3] = bits[3];
                bitsN[4] = bits[4];
                bitsN[5] = bits[5];
                bitsN[6] = bits[6];

                int[] intN = new int[1];
                bitsN.CopyTo(intN, 0);

                if (intN[0] == 0)
                {
                    FormMain.EventSend.AppendTextBox("ASDU не содержит информационных объектов");
                }
                else
                {
                    if (bits[7] == false)
                    {
                        FormMain.EventSend.AppendTextBox("Число информационных объектов: " + intN[0].ToString());
                    }
                    else
                    {
                        FormMain.EventSend.AppendTextBox("Число информационных элементов: " + intN[0].ToString());
                    }
                }               
            }

            // Причины передачи
            if (lenght >=7)
            {
                BitArray bits = GetBitsOfByte(bytes[6]);

                BitArray bitNumberTransferReason = new BitArray(6, false);
                bitNumberTransferReason[0] = bits[0];
                bitNumberTransferReason[1] = bits[1];
                bitNumberTransferReason[2] = bits[2];
                bitNumberTransferReason[3] = bits[3];
                bitNumberTransferReason[4] = bits[4];
                bitNumberTransferReason[5] = bits[5];

                int[] intNumberTransferReason = new int[1];
                bitNumberTransferReason.CopyTo(intNumberTransferReason, 0);
                FormMain.EventSend.AppendTextBox("Причина передачи: " + ASDUinfo.TransferReason(intNumberTransferReason[0]));

                if (bits[6] == false)
                {
                    FormMain.EventSend.AppendTextBox("положительное подтверждение");
                }
                else
                {
                    FormMain.EventSend.AppendTextBox("отрицательное подтверждение");
                }

                if (bits[7] == false)
                {
                    FormMain.EventSend.AppendTextBox("не тест");
                }
                else
                {
                    FormMain.EventSend.AppendTextBox("тест");
                }              
            }
            if (lenght >= 8)
            {
                FormMain.EventSend.AppendTextBox("Адрес инициируещей станции: " + GetIntOfBytes(bytes[7]));
            }
            
            // Общий адрес ASDU          
            if (lenght >= 9)
            {
                int[] adressASDU = new int[1];
                BitArray bitsASDU = new BitArray(16, false);                
                
                BitArray bits1 = GetBitsOfByte(bytes[8]);

                bitsASDU[0] = bits1[0];
                bitsASDU[1] = bits1[1];
                bitsASDU[2] = bits1[2];
                bitsASDU[3] = bits1[3];
                bitsASDU[4] = bits1[4];
                bitsASDU[5] = bits1[5];
                bitsASDU[6] = bits1[6];
                bitsASDU[7] = bits1[7];

                if (lenght >= 10)
                {
                    BitArray bits2 = GetBitsOfByte(bytes[9]);

                    bitsASDU[8] = bits2[0];
                    bitsASDU[9] = bits2[1];
                    bitsASDU[10] = bits2[2];
                    bitsASDU[11] = bits2[3];
                    bitsASDU[12] = bits2[4];
                    bitsASDU[13] = bits2[5];
                    bitsASDU[14] = bits2[6];
                    bitsASDU[15] = bits2[7];
                }

                bitsASDU.CopyTo(adressASDU, 0);

                FormMain.EventSend.AppendTextBox("Общий адрес ASDU: " + adressASDU[0].ToString());
            }
            
            // Адрес информационного объекта
            if (lenght >= 11)
            {
                int[] adressObject = new int[1];
                BitArray bitObject = new BitArray(24, false);                
                
                BitArray bits1 = GetBitsOfByte(bytes[10]);

                bitObject[0] = bits1[0];
                bitObject[1] = bits1[1];
                bitObject[2] = bits1[2];
                bitObject[3] = bits1[3];
                bitObject[4] = bits1[4];
                bitObject[5] = bits1[5];
                bitObject[6] = bits1[6];
                bitObject[7] = bits1[7];

                if (lenght >= 12)
                {
                    BitArray bits2 = GetBitsOfByte(bytes[11]);

                    bitObject[8] = bits2[0];
                    bitObject[9] = bits2[1];
                    bitObject[10] = bits2[2];
                    bitObject[11] = bits2[3];
                    bitObject[12] = bits2[4];
                    bitObject[13] = bits2[5];
                    bitObject[14] = bits2[6];
                    bitObject[15] = bits2[7];

                    if (lenght >= 13)
                    {
                        BitArray bits3 = GetBitsOfByte(bytes[12]);

                        bitObject[16] = bits2[0];
                        bitObject[17] = bits2[1];
                        bitObject[18] = bits2[2];
                        bitObject[19] = bits2[3];
                        bitObject[20] = bits2[4];
                        bitObject[21] = bits2[5];
                        bitObject[22] = bits2[6];
                        bitObject[23] = bits2[7];
                    }
                }
                bitObject.CopyTo(adressObject, 0);

                FormMain.EventSend.AppendTextBox("Адрес информационного объекта: " + adressObject[0].ToString());
            }
        }
    }
}
