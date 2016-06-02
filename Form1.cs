using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ListenerIEC104
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBoxConsole.AppendText(value + Environment.NewLine);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void SocketListen()
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
                    //Console.Write("Waiting for a connection... ");
                    AppendTextBox("Waiting for a connection...");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    //Console.WriteLine("Connected!");
                    AppendTextBox("Connected!");

                    //data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = DecoderIEC104.IECToString(bytes);
                        AppendTextBox(data);

                        data = null;
                        
                        /*foreach (byte b in bytes)
                        {
                            data = data + Convert.ToString(b, 2).PadLeft(8, '0') + " ";
                        }

                        AppendTextBox("Received: ");
                        AppendTextBox(data);

                        string start68H_IEC104 = Convert.ToString(bytes[0], 2).PadLeft(8, '0') + " ";
                        int start68H_int32 = Convert.ToInt32(bytes[0]);
                        AppendTextBox("Старт 68H: " + Environment.NewLine + start68H_IEC104 + " - " + start68H_int32.ToString() + Environment.NewLine);

                        string leghtASDU_IEC104 = Convert.ToString(bytes[1], 2).PadLeft(8, '0') + " ";
                        int leghtASDU_int32 = Convert.ToInt32(bytes[1]);
                        AppendTextBox("Длинна APDU: " + Environment.NewLine + leghtASDU_IEC104 + " - " + leghtASDU_int32.ToString() + Environment.NewLine);

                        data = null;*/

                        // Process the data sent by the client.
                        //data = data.ToUpper();

                        //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

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
                AppendTextBox("Error");
            }
            finally
            {
                // Stop listening for new clients.                
                server.Stop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxConsole.Clear();
            textBoxConsole.AppendText("Start" + Environment.NewLine);
            Application.DoEvents();

            GlobalVar.threadingRun = true;
            GlobalVar.port = 2405;
            Thread SocketListenThread = new Thread(SocketListen);
            SocketListenThread.Start();
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GlobalVar.threadingRun = false;
        }
    }
}
