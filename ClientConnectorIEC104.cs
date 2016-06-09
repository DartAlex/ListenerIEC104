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
                        data = "Received: " + Environment.NewLine + DecoderIEC104.IECToString(bytes) + Environment.NewLine;
                        FormMain.EventSend.AppendTextBox(data);
                        DecoderIEC104.ReadAPCI(bytes);

                        data = null;

                        // Process the data sent by the client.

                        byte b1 = Convert.ToByte(104);
                        byte b2 = Convert.ToByte(4);
                        byte b3 = Convert.ToByte(11);
                        byte b4 = Convert.ToByte(0);
                        byte b5 = Convert.ToByte(0);
                        byte b6 = Convert.ToByte(0);

                        byte[] answer = new byte[6] { b1, b2, b3, b4, b5, b6 };

                        data = "Transmitted: " + Environment.NewLine + DecoderIEC104.IECToString(answer) + Environment.NewLine;
                        FormMain.EventSend.AppendTextBox(data);

                        data = null;

                        // Send back a response.
                        stream.Write(answer, 0, answer.Length);
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
