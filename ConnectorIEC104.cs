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
        public static async void SocketListen()
        {           
            TcpListener tcpListener = null;
            try
            {
                Int32 port = GlobalVar.port;
                IPAddress localAddr = Dns.GetHostAddresses(Dns.GetHostName())[1];

                tcpListener = new TcpListener(localAddr, port);
                tcpListener.Start();
              
                while (GlobalVar.threadingRun == true)
                {
                    try
                    {
                        FormMain.EventSend.AppendServerConsole("Waiting for a connection...");
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        FormMain.EventSend.AppendServerConsole("Connected!");

                        NetworkStream stream = tcpClient.GetStream();

                        while (tcpClient.Connected && GlobalVar.threadingRun == true)
                        {

                            byte[] bytes = new byte[255];

                            await stream.ReadAsync(bytes, 0, bytes.Length);
                            bytes = DecoderIEC104.ParseLengthPacket(bytes);
                            ShowServerReceived(bytes);
                            byte[] temp = DecoderIEC104.Read(bytes);

                            byte[] answer = SendData(bytes);
                            ShowServerTransmitted(answer);
                            byte[] temp2 = DecoderIEC104.Read(answer);
                            await stream.WriteAsync(answer, 0, answer.Length);

                        }
                        // Shutdown and end connection
                        tcpClient.Close();
                        FormMain.EventSend.AppendServerConsole("Close connection" + Environment.NewLine);
                    }
                    catch(Exception e)
                    {
                        FormMain.EventSend.AppendServerConsole("Server close connection" + Environment.NewLine);
                        //CloseConnection();
                    }                                   
                }
            }
            catch(Exception e)
            {
                FormMain.EventSend.AppendServerConsole("From serever: " + e.ToString() + Environment.NewLine);
            }
            finally
            {
                // Stop listening for new clients.                
                tcpListener.Stop();
                FormMain.EventSend.AppendServerConsole("Server stop" + Environment.NewLine);
            }
        }

        // Create a TCP/IP  socket.
        private static Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void ConnectCallback(IAsyncResult ar)
        {
            allDone.Set();
            Socket s = (Socket)ar.AsyncState;
            s.EndConnect(ar);
        }

        public static void Connect()
        {
            //CloseConnection();
            if (sender.Connected) return;

            byte[] bytes = new byte[255];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                IPAddress ipAddress = IPAddress.Parse("ip adress");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.BeginConnect(ipAddress, 2458, new AsyncCallback(ConnectCallback), sender);
                    FormMain.EventSend.AppendClientConsole("Socket connected to " + sender.RemoteEndPoint.ToString());
                    allDone.WaitOne();

                    
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

        public static byte[] SendData(byte[] bytes)
        {
            FormMain.EventSend.AppendClientConsole("Connected " + sender.Connected.ToString());

            if (sender.Connected == false)
            {
                Connect();
            }

            byte[] answer = new byte[255];

            // Send the data through the socket.
            int bytesSent = sender.Send(bytes);
            ShowClientTransmitted(bytes);
            // Receive the response from the remote device.
            int bytesRec = sender.Receive(answer);           
            answer = DecoderIEC104.ParseLengthPacket(answer);

            ShowClientReceived(answer);
            return (answer);
        }

        private static void CloseConnection()
        {
            try
            {
                // Release the socket.
                //sender.Shutdown(SocketShutdown.Both);
                //sender.Close();
                sender.Dispose();
            }
            catch { }
        }
    }
}
