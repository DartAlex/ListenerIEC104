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
        public static FormMain EventSend;
        
        public FormMain()
        {
            InitializeComponent();
            EventSend = this;
        }

        public void AppendClientConsole(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendClientConsole), new object[] { value });
                return;
            }
            ClientConsole.AppendText(Environment.NewLine + value);
        }

        public void AppendServerConsole(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendServerConsole), new object[] { value });
                return;
            }
            ServerConsole.AppendText(Environment.NewLine + value);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerConsole.Clear();
            ServerConsole.AppendText("Start");
            Application.DoEvents();

            GlobalVar.threadingRun = true;
            GlobalVar.port = 2405;
            //Thread SocketListenThread = new Thread(Server.AsynchronousSocketListener.StartListening);
            Thread SocketListenThread = new Thread(ConnectorIEC104.SocketListen);
            //Thread SocketListenThread = new Thread(Client.RunServer);
            SocketListenThread.Start();
            //ConnectorIEC104.SocketListen();
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GlobalVar.threadingRun = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ConnectorIEC104.Connect();
            //ConnectorIEC104.BeginConnect();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ConnectorIEC104.SendData(EncoderIEC104.FormatUStartAct());           
        }
    }
}
