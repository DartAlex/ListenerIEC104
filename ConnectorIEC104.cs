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
        // Connect as client
        public static async void SocketListen()
        {           
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port
                Int32 port = GlobalVar.port;
                // Buffer for reading data
                //Byte[] bytes = new Byte[255];

                IPAddress localAddr = Dns.GetHostAddresses(Dns.GetHostName())[1];

                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
              
                // Enter the listening loop.
                while (GlobalVar.threadingRun == true)
                {
                    try
                    {
                        FormMain.EventSend.AppendTextBox("Waiting for a connection...");

                        // Perform a blocking call to accept requests.
                        TcpClient client = server.AcceptTcpClient();
                        FormMain.EventSend.AppendTextBox("Connected!");

                        // Get a stream object for reading and writing
                        //NetworkStream stream = client.GetStream();

                        //int i;
                        // Loop to receive all the data sent by the client.
                        /*while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                        {                           
                            //data = Environment.NewLine + "Received: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine;
                            //FormMain.EventSend.AppendTextBox(data);

                            // Send back a response.
                            //byte[] answer = EncoderIEC104.FormatUStartCon();
                            //byte[] answer = DecoderIEC104.ReadAPCI(bytes);
                            //var temp = DecoderIEC104.ReadAPCI(bytes);
                            
                            byte[] answer = SendData(bytes);

                            await stream.WriteAsync(answer, 0, answer.Length);

                            //data = Environment.NewLine + "Transmitted: " + Environment.NewLine + DecoderIEC104.IECToString(answer) + Environment.NewLine;
                            //FormMain.EventSend.AppendTextBox(data);

                            //var tempAnswer = DecoderIEC104.ReadAPCI(answer);
                        }*/

                        while (client.Connected && GlobalVar.threadingRun == true)
                        {

                            // Get a stream object for reading and writing
                            NetworkStream stream = client.GetStream();

                            byte[] bytes = new byte[255];

                            await stream.ReadAsync(bytes, 0, bytes.Length);
                            string data = Environment.NewLine + "Received: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine;
                            FormMain.EventSend.AppendTextBox(data);

                            if (DecoderIEC104.PacketLenght(bytes) > 0)
                            {
                                //DecoderIEC104.ReadAPCI(bytes);
                                //byte[] answer = SendData(bytes);
                                //byte[] answer = DecoderIEC104.ReadAPCI(bytes);
                                //data = Environment.NewLine + "Transmitted: " + Environment.NewLine + DecoderIEC104.IECToString(answer) + Environment.NewLine;
                                FormMain.EventSend.AppendTextBox(data);
                                //DecoderIEC104.ReadAPCI(answer);

                                //await stream.WriteAsync(answer, 0, answer.Length);
                            }
                            else
                            {
                                client.Close();
                                //CloseConnection();
                            }
                            //DecoderIEC104.ReadAPCI(bytes);
                            //byte[] answer = SendData(bytes);                                                       
                        }

                        // Shutdown and end connection
                        //sender.Close();
                        client.Close();
                        //CloseConnection();
                        FormMain.EventSend.AppendTextBox("Close connection" + Environment.NewLine);
                    }
                    catch(Exception e)
                    {
                        FormMain.EventSend.AppendTextBox("Server close connection" + Environment.NewLine);
                        //CloseConnection();
                    }                                   
                }
            }
            catch(Exception e)
            {
                FormMain.EventSend.AppendTextBox("From serever: " + e.ToString() + Environment.NewLine);
            }
            finally
            {
                // Stop listening for new clients.                
                server.Stop();
                FormMain.EventSend.AppendTextBox("Server stop" + Environment.NewLine);
            }
        }

        // Create a TCP/IP  socket.
        private static Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void Connect()
        {
            //CloseConnection();
            if (sender.Connected) return;

            byte[] bytes = new byte[255];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                IPAddress ipAddress = IPAddress.Parse(ip adress);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);


                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);
                    FormMain.EventSend.AppendTextBox("Socket connected to " + sender.RemoteEndPoint.ToString());                                                                       
                }
                catch (ArgumentNullException ane)
                {
                    FormMain.EventSend.AppendTextBox("Server ArgumentNullException : " + ane.ToString() + Environment.NewLine);
                }
                catch (SocketException se)
                {
                    FormMain.EventSend.AppendTextBox("Server SocketException : " + se.ToString() + Environment.NewLine);
                }
                catch (Exception e)
                {
                    FormMain.EventSend.AppendTextBox("Server Unexpected exception : " + e.ToString() + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                FormMain.EventSend.AppendTextBox(e.ToString());               
            }
        }

        public static byte[] SendData(byte[] bytes)
        {
            FormMain.EventSend.AppendTextBox(sender.Connected.ToString());

            if (sender.Connected == false)
            {
                Connect();
            }
            
            byte[] answer = new byte[255];

            // Send the data through the socket.
            int bytesSent = sender.Send(bytes);
            string data = Environment.NewLine + "Received: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine;
            FormMain.EventSend.AppendTextBox(data);
            //DecoderIEC104.ReadAPCI(bytes);
            
            // Receive the response from the remote device.
            int bytesRec = sender.Receive(answer);          

            data = Environment.NewLine + "Transmitted: " + Environment.NewLine + DecoderIEC104.IECToString(answer) + Environment.NewLine;
            FormMain.EventSend.AppendTextBox(data);
            //DecoderIEC104.ReadAPCI(answer);

            return (answer);
        }

        private static void CloseConnection()
        {
            try
            {
                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch { }
             
        }

        // Connect as server
        public static byte[] SocketConnect(byte[] answer)
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[255];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                IPAddress ipAddress = IPAddress.Parse("172.22.143.226");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2494);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);
                    FormMain.EventSend.AppendTextBox("Socket connected to " + sender.RemoteEndPoint.ToString());                   
                  
                    // Send the data through the socket.
                    int bytesSent = sender.Send(answer);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();                                                       
                }
                catch (ArgumentNullException ane)
                {
                    FormMain.EventSend.AppendTextBox("Server ArgumentNullException : " + ane.ToString() + Environment.NewLine);
                }
                catch (SocketException se)
                {
                    FormMain.EventSend.AppendTextBox("Server SocketException : " + se.ToString() + Environment.NewLine);
                }
                catch (Exception e)
                {
                    FormMain.EventSend.AppendTextBox("Server Unexpected exception : " + e.ToString() + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                FormMain.EventSend.AppendTextBox(e.ToString());               
            }
            return (bytes);
        }
    }
}
