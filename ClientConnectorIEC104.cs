using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ListenerIEC104
{
    class ClientConnectorIEC104
    {
        public static void SocketListen()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port
                Int32 port = GlobalVar.port;

                IPAddress localAddr = Dns.GetHostAddresses(Dns.GetHostName())[1];

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[255];
                String data = null;

                // Enter the listening loop.
                //while (true)
                while (GlobalVar.threadingRun == true)
                {
                    FormMain.EventSend.AppendTextBox("Waiting for a connection...");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    FormMain.EventSend.AppendTextBox("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = DecoderIEC104.IECToString(bytes);
                        FormMain.EventSend.AppendTextBox(data);

                        data = null;

                        // Process the data sent by the client.

                        //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        //byte[] answer = BitConverter.GetBytes(bytes);

                        // Send back a response.
                        //stream.Write(msg, 0, msg.Length);

                        /*stream.Write(bytes, 0, bytes.Length);
                        foreach (byte b in bytes)
                        {
                            data = data + b.ToString();
                        }

                        Console.WriteLine("Sent: {0}", data);

                        data = null;*/

                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch
            {
                FormMain.EventSend.AppendTextBox("Error");
            }
            finally
            {
                // Stop listening for new clients.                
                server.Stop();
            }
        }
    }
}
