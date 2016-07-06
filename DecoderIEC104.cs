using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ListenerIEC104
{
    class APCIStruct
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

    class TransferReasonStruct
    {
        public string reason { get; set; }
        public bool confirmation { get; set; }
        public bool test { get; set; }
    }

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
        /*public static string IECToString(byte[] bytes)
        {
            string data = null;

            for (int i = 0; i <= PacketLenght(bytes) + 1; i++)
            {
                data = data + Convert.ToString(bytes[i], 2).PadLeft(8, '0') + " ";
            }
            return (data);
        }*/

        // To String
        public static string IECToString(byte[] bytes)
        {
            string data = null;

            for (int i = 0; i <= bytes.Length-1; i++)
            {
                data = data + Convert.ToString(bytes[i], 2).PadLeft(8, '0') + " ";
            }
            return (data);
        }



        public static byte[] ParseLengthPacket(byte[] bytes)
        {
            int lengthPacket = Convert.ToInt32(bytes[1]) + 2;

            byte[] result = new byte[lengthPacket];
            
            for (int i = 0; i < lengthPacket; i++)
            {
                result[i] = bytes[i];
            }
            return(result);
        }

        public static byte[] Read(byte[] bytes)
        {
            byte[] answer = null;
            byte[] bytesAPCI = new byte[] 
            { 
                bytes[0],
                bytes[1],
                bytes[2],
                bytes[3],
                bytes[4],
                bytes[5]
            };

            APCIStruct apci = ReadAPCI(bytesAPCI);
          
            if (apci.headerByte == true)
            {
                FormMain.EventSend.AppendServerConsole("Protocol is IEC104");
                FormMain.EventSend.AppendServerConsole("Lenght APDU: " + apci.lenghtAPDU);
                
                if (apci.formatI == true)
                {
                    FormMain.EventSend.AppendServerConsole("Format I; N(S) = " + apci.nS + "; N(R) = " + apci.nR);
                    ReadASDU(bytes, apci.lenghtAPDU);
                }

                if (apci.formatS == true)
                {
                    FormMain.EventSend.AppendServerConsole("Format S; N(R) = " + apci.nR);
                    ReadASDU(bytes, apci.lenghtAPDU);
                }

                if (apci.formatU == true)
                {
                    if (apci.startDT == true)
                    {
                        if (apci.act == true)
                        {
                            FormMain.EventSend.AppendServerConsole("Format U STARTDT act - Старт передачи данных. Активация.");
                            answer = EncoderIEC104.FormatUStartCon();
                        }

                        if (apci.con == true)
                        {
                            FormMain.EventSend.AppendServerConsole("Format U STARTDT con - Старт передачи данных. Подтверждение.");
                        }
                    }

                    if (apci.stopDT == true)
                    {
                        if (apci.act == true)
                        {
                            FormMain.EventSend.AppendServerConsole("Format U STOPDT act - Прекращение передачи данных. Активация.");
                            answer = EncoderIEC104.FormatUStopCon();
                        }

                        if (apci.con == true)
                        {
                            FormMain.EventSend.AppendServerConsole("Format U STOPDT con - Прекращение передачи данных. Подтверждение.");
                        }
                    }

                    if (apci.testFR == true)
                    {
                        if (apci.act == true)
                        {
                            FormMain.EventSend.AppendServerConsole("Format U TESTFR act - Тестовый блок. Активация.");
                            answer = EncoderIEC104.FormatUTestCon();
                        }

                        if (apci.con == true)
                        {
                            FormMain.EventSend.AppendServerConsole("Format U TESTFR con - Тестовый блок. Подтверждение.");
                        }
                    }
                }
            }
            return (answer);
        }

        private static APCIStruct ReadAPCI(byte[] bytes)
        {
            APCIStruct apci = new APCIStruct() 
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
                }
                // Format S
                if (bitsField1[0] == true && bitsField1[1] == false)
                {
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
                    }
                    // STARTDT con - Старт передачи данных. Подтверждение.
                    if (startDtCon == true)
                    {
                        apci.startDT = true;
                        apci.con = true;
                    }
                    // STOPDT act - Прекращение передачи данных. Активация.
                    if (stopDtAct == true)
                    {
                        apci.stopDT = true;
                        apci.act = true;
                    }
                    // STOPDT con - Прекращение передачи данных. Подтверждение.
                    if (stopDtCon == true)
                    {
                        apci.stopDT = true;
                        apci.con = true;
                    }
                    // TESTFR act - Тестовый блок. Активация.
                    if (testFrAct == true)
                    {
                        apci.testFR = true;
                        apci.act = true;
                    }
                    // "TESTFR con - Тестовый блок. Подтверждение."
                    if (testFrCon == true)
                    {
                        apci.testFR = true;
                        apci.con = true;
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
        private static void ReadASDU(byte[] bytes, int packetLenght)
        {
            string log;

            // Индетификатор типов
            int intType = GetIntOfBytes(bytes[6]);
            log = "Индетификатор типов: " + ASDUinfo.TypeIdentifier(intType);
            FormMain.EventSend.AppendServerConsole(log);

            // Классификатор переменной структуры
            BitArray bits7 = GetBitsOfByte(bytes[7]);

            BitArray bitsN = new BitArray(7, false);
            bitsN[0] = bits7[0];
            bitsN[1] = bits7[1];
            bitsN[2] = bits7[2];
            bitsN[3] = bits7[3];
            bitsN[4] = bits7[4];
            bitsN[5] = bits7[5];
            bitsN[6] = bits7[6];

            int[] intN = new int[1];
            bitsN.CopyTo(intN, 0);

            if (intN[0] == 0)
            {
                log = "ASDU не содержит информационных объектов";

            }
            else
            {
                if (bits7[7] == false)
                {
                    log = "Число информационных объектов: " + intN[0].ToString();
                }
                else
                {
                    log = "Число информационных элементов: " + intN[0].ToString();
                }
            }

            FormMain.EventSend.AppendServerConsole(log);

            // Причины передачи
            TransferReasonStruct transferReasonStruct = TransferReason(bytes[8]);
            log = "Причины передачи: " + transferReasonStruct.reason;
            if (transferReasonStruct.confirmation == true)
            {
                log = log + ", положительное подтверждение";
            }
            else
            {
                log = log + ", отрицательное подтверждение";
            }
            if (transferReasonStruct.test == true)
            {
                log = log + ", тест";
            }
            else
            {
                log = log + ", не тест";
            }
            FormMain.EventSend.AppendServerConsole(log);

            // Номер инициирующего адреса
            FormMain.EventSend.AppendServerConsole("Номер инициирующего адреса: " + GetIntOfBytes(bytes[9]));

            // Общий адрес ASDU
            byte[] byteGeneralAddressASDU = new byte[] { bytes[10], bytes[11] };
            int intGeneralAddressASDU = GeneralAddressASDU(byteGeneralAddressASDU);
            FormMain.EventSend.AppendServerConsole("Общий адрес ASDU: " + intGeneralAddressASDU.ToString());

            // Объекты информации
            List<byte> byteObjectInformation = new List<byte>();
            for (int i = 12; i < (packetLenght + 2); i++)
            {
                byteObjectInformation.Add(bytes[i]);
            }
            FormMain.EventSend.AppendServerConsole(ObjectsInformation(byteObjectInformation, ASDUinfo.TypeIdentifier(intType)));          
        }

        private static string ObjectsInformation(List<byte> listBytes, string identifierTypes)
        {
            string data = identifierTypes;
            byte[] byteAddressInformationObject;
            switch(identifierTypes)
            {
                // 45
                case "C_SC_NA_1":
                    data = "одноэлементная команда C_SC_NA_1";
                    // Адрес информационного объекта

                    byteAddressInformationObject = new byte[] { listBytes[0], listBytes[1], listBytes[2] };
                    data = data + Environment.NewLine + "Адрес информационного объекта: " + AddressInformationObject(byteAddressInformationObject).ToString();

                    BitArray bitSCO = GetBitsOfByte(listBytes[3]);

                    if (bitSCO[0] == true)
                    {
                        data = data + Environment.NewLine + "однопозиционная команда: вкл";
                    }
                    else
                    {
                        data = data + Environment.NewLine + "однопозиционная команда: выкл";
                    }

                    BitArray bitQU = new BitArray(5, false);
                    bitQU[0] = bitSCO[2];
                    bitQU[1] = bitSCO[3];
                    bitQU[2] = bitSCO[4];
                    bitQU[3] = bitSCO[5];
                    bitQU[4] = bitSCO[6];
                    

                    int[] intQU = new int[1];
                    bitQU.CopyTo(intQU, 0);

                    data = data + Environment.NewLine + "QU: " + ASDUinfo.QU(intQU[0]);                    

                    if (bitSCO[7] == true)
                    {
                        data = data + Environment.NewLine + "S/E исполнение";
                    }
                    else
                    {
                        data = data + Environment.NewLine + "S/E выбор";
                    }
                    break;
                
                // 100
                case "C_IC_NA_1":
                    data = "команда опроса C_IC_NA_1";
                    // Адрес информационного объекта
                    byteAddressInformationObject = new byte[] { listBytes[0], listBytes[1], listBytes[2] };
                    data = data + Environment.NewLine + "Адрес информационного объекта: " + AddressInformationObject(byteAddressInformationObject).ToString();

                    int intQOI = GetIntOfBytes(listBytes[3]);
                    data = data + Environment.NewLine + ASDUinfo.QOI(intQOI);
                    break;                
            }
            return (data);
        }

        private static TransferReasonStruct TransferReason(byte byteTransferReason)
        {
            TransferReasonStruct transferReasonStruct = new TransferReasonStruct();

            BitArray bits = GetBitsOfByte(byteTransferReason);

            BitArray bitTransferReason = new BitArray(6, false);
            bitTransferReason[0] = bits[0];
            bitTransferReason[1] = bits[1];
            bitTransferReason[2] = bits[2];
            bitTransferReason[3] = bits[3];
            bitTransferReason[4] = bits[4];
            bitTransferReason[5] = bits[5];

            int[] intTransferReason = new int[1];
            bitTransferReason.CopyTo(intTransferReason, 0);

            transferReasonStruct.reason = ASDUinfo.TransferReason(intTransferReason[0]);


            if (bits[6] == false)
            {
                transferReasonStruct.confirmation = true;
            }
            else
            {
                transferReasonStruct.confirmation = false;
            }

            if (bits[7] == false)
            {
                transferReasonStruct.test = false;
            }
            else
            {
                transferReasonStruct.test = true;
            }

            return (transferReasonStruct);
        }

        private static int GeneralAddressASDU(byte[] bytes)
        {
            int[] adressASDU = new int[1];
            BitArray bitsASDU = new BitArray(16, false);

            BitArray bits10 = GetBitsOfByte(bytes[0]);

            bitsASDU[0] = bits10[0];
            bitsASDU[1] = bits10[1];
            bitsASDU[2] = bits10[2];
            bitsASDU[3] = bits10[3];
            bitsASDU[4] = bits10[4];
            bitsASDU[5] = bits10[5];
            bitsASDU[6] = bits10[6];
            bitsASDU[7] = bits10[7];

            BitArray bits11 = GetBitsOfByte(bytes[1]);

            bitsASDU[8] = bits11[0];
            bitsASDU[9] = bits11[1];
            bitsASDU[10] = bits11[2];
            bitsASDU[11] = bits11[3];
            bitsASDU[12] = bits11[4];
            bitsASDU[13] = bits11[5];
            bitsASDU[14] = bits11[6];
            bitsASDU[15] = bits11[7];

            bitsASDU.CopyTo(adressASDU, 0);
            return (adressASDU[0]);
            
        }
        
        private static int AddressInformationObject(byte[] bytes)
        {
            int[] adressObject = new int[1];
            BitArray bitObject = new BitArray(24, false);

            BitArray bits1 = GetBitsOfByte(bytes[0]);

            bitObject[0] = bits1[0];
            bitObject[1] = bits1[1];
            bitObject[2] = bits1[2];
            bitObject[3] = bits1[3];
            bitObject[4] = bits1[4];
            bitObject[5] = bits1[5];
            bitObject[6] = bits1[6];
            bitObject[7] = bits1[7];

            BitArray bits2 = GetBitsOfByte(bytes[1]);

            bitObject[8] = bits2[0];
            bitObject[9] = bits2[1];
            bitObject[10] = bits2[2];
            bitObject[11] = bits2[3];
            bitObject[12] = bits2[4];
            bitObject[13] = bits2[5];
            bitObject[14] = bits2[6];
            bitObject[15] = bits2[7];

            BitArray bits3 = GetBitsOfByte(bytes[2]);

            bitObject[16] = bits3[0];
            bitObject[17] = bits3[1];
            bitObject[18] = bits3[2];
            bitObject[19] = bits3[3];
            bitObject[20] = bits3[4];
            bitObject[21] = bits3[5];
            bitObject[22] = bits3[6];
            bitObject[23] = bits3[7];

            return (adressObject[0]);           
        }
    }
}
