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

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBoxConsole.AppendText(Environment.NewLine + value);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxConsole.Clear();
            textBoxConsole.AppendText("Start");
            Application.DoEvents();

            GlobalVar.threadingRun = true;
            GlobalVar.port = 2405;
            Thread SocketListenThread = new Thread(ClientConnectorIEC104.SocketListen);
            SocketListenThread.Start();
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GlobalVar.threadingRun = false;
        }
    }
}
