using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ListenerIEC104
{
    class ConnectorIEC104
    {  
        private static void ShowServerReceived(byte[] bytes)
        {
            FormMain.EventSend.AppendServerConsole(Environment.NewLine + "Received: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine);
        }
        private static void ShowServerTransmitted(byte[] bytes)
        {
            FormMain.EventSend.AppendServerConsole(Environment.NewLine + "Transmitted: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine);
        }

        private static void ShowClientReceived(byte[] bytes)
        {
            FormMain.EventSend.AppendClientConsole(Environment.NewLine + "Received: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine);
        }
        private static void ShowClientTransmitted(byte[] bytes)
        {
            FormMain.EventSend.AppendClientConsole(Environment.NewLine + "Transmitted: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine);
        }

        // Connect as client
        public static void SocketListen()
        {
            TcpListener tcpListener = null;
            try
            {
                Int32 port = GlobalVar.portListen;
                IPAddress localAddr = Dns.GetHostAddresses(Dns.GetHostName())[1];

                tcpListener = new TcpListener(localAddr, port);
                tcpListener.Start();

                while (GlobalVar.threadingRun == true)
                {
                    Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        Connect(sender);
                        
                        FormMain.EventSend.AppendServerConsole("Waiting for a connection...");
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        FormMain.EventSend.AppendServerConsole("Connected!");

                        NetworkStream stream = tcpClient.GetStream();

                        while (tcpClient.Connected && sender.Connected && GlobalVar.threadingRun == true)
                        {

                            byte[] bytes = new byte[255];
                            // recerved
                            stream.Read(bytes, 0, bytes.Length);
                            bytes = DecoderIEC104.ParseLengthPacket(bytes);
                            ShowServerReceived(bytes);

                            if (DecoderIEC104.HeaderPacket(bytes[0]))
                            {
                                FormMain.EventSend.AppendServerConsole(DecoderIEC104.Read(bytes));
                                // tranclate
                                byte[] answer = SendData(bytes, sender);
                                ShowServerTransmitted(answer);
                                FormMain.EventSend.AppendServerConsole(DecoderIEC104.Read(answer));
                                stream.Write(answer, 0, answer.Length);
                            }
                            else
                            {
                                break;
                            }                         
                        }
                        // Shutdown and end connection
                        sender.Close();
                        tcpClient.Close();
                        FormMain.EventSend.AppendServerConsole("Close connection" + Environment.NewLine);
                        FormMain.EventSend.AppendClientConsole("Close connection" + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        FormMain.EventSend.AppendServerConsole("Server close connection " + e.ToString() + Environment.NewLine);
                        sender.Close();
                        FormMain.EventSend.AppendClientConsole("Close connection" + Environment.NewLine);
                    }
                }
            }
            catch (Exception e)
            {
                FormMain.EventSend.AppendServerConsole("From serever: " + e.ToString() + Environment.NewLine);
            }
            finally
            {               
                tcpListener.Stop();
                FormMain.EventSend.AppendServerConsole("Server stop" + Environment.NewLine);
            }
        }

        public static void Connect(Socket sender)
        {
            // Connect to a remote device.
            try
            {
                IPAddress ipAddress = IPAddress.Parse(GlobalVar.ipSender);
                Int32 port = GlobalVar.portSender;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                try
                {
                    sender.Connect(remoteEP);
                    FormMain.EventSend.AppendClientConsole("Socket connected to " + sender.RemoteEndPoint.ToString());               
                }
                catch (ArgumentNullException ane)
                {
                    FormMain.EventSend.AppendClientConsole("Server ArgumentNullException : " + ane.ToString() + Environment.NewLine);
                }
                catch (SocketException se)
                {
                    FormMain.EventSend.AppendClientConsole("Server SocketException : " + se.ToString() + Environment.NewLine);
                }
                catch (Exception e)
                {
                    FormMain.EventSend.AppendClientConsole("Server Unexpected exception : " + e.ToString() + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                FormMain.EventSend.AppendClientConsole(e.ToString());
            }
        }

        public static byte[] SendData(byte[] bytes, Socket sender)
        {
            byte[] answer = new byte[255];

            int bytesSent = sender.Send(bytes);
            ShowClientTransmitted(bytes);

            int bytesRec = sender.Receive(answer);
            answer = DecoderIEC104.ParseLengthPacket(answer); 

            ShowClientReceived(answer);
            return (answer);
        }
    }
}
