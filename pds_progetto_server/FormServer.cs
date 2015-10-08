using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseKeyboardLibrary;

/* Server */

namespace pds_progetto_server
{
    enum ServerStatus : byte
    {
        Off,
        Listenining,
        ClientConnected,
        ClientActive
    }

    public partial class FormServer : Form
    {
        private ServerSocket ss = new ServerSocket();
        volatile ServerStatus status;

        private ServerKeyboardHook serverKeyboardHook = new ServerKeyboardHook();
        private ServerMouseHook serverMouseHook = new ServerMouseHook();

        public FormServer()
        {
            InitializeComponent();
            status = ServerStatus.Off;
            RefreshStatusLabel();
            ss.ClientConnected += OnClientConnected;       // evento: client si collega al ServerSocket
            ss.ClientDisconnected += OnClientDisconnected;    // evento: client di disconnette dal ServerSocket
            ss.ClientSendEventsStarted += OnReceivingEvents;    // evento: client inizia ad inviare eventi al ServerSocket
            ss.ClientSendEventsEnded += OnClientStoppedSendingEvent;    // evento: client smette di inviare eventi al ServerSocket
        }

        private void btnStop_Click(object sender, EventArgs e)      // stop listening
        {
            if(status == ServerStatus.Listenining || status == ServerStatus.ClientConnected)
            {
                ss.StopListening();
                status = ServerStatus.Off;
                RefreshStatusLabel();
                btnStart.Enabled = true;                 
                serverKeyboardHook.Stop();
                serverMouseHook.Stop();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)     // start listening
        {
            if (status != ServerStatus.Off) return;

            waitClient();
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            serverKeyboardHook.Start();
            serverMouseHook.Start();
        }

        private void waitClient()       // ServerSocket va in listening
        {
            //produttore: riceve gli eventi da socket
            Task.Factory.StartNew(() =>
            {
                status = ServerStatus.Listenining;
                RefreshStatusLabel();
                ss.StartListening(Int32.Parse(this.txtPortNumber.Text));                
            });
        }

        private void OnClientConnected(object sender, EventArgs e)     // chiamata quando il client si connette
        {
            status = ServerStatus.ClientConnected;
            RefreshStatusLabel();
            
            /* gli hook vanno attivati quando il client inizia a mandare eventi! */
            //serverKeyboardHook.Enabled = true;
            //serverMouseHook.Enabled = true;

            /* qua o dopo che il client inizia a mandare eventi? */
            // consumatore: riproduce gli eventi
            //Task.Factory.StartNew(() =>
            //{
                //while (connected)
            //    while(status == ServerStatus.ClientConnected || status == ServerStatus.ClientActive)
            //        EventsBuffer.ConsumeEvents();       // si blocca in attesa del primo evento ricevuto
            //});
        }

        private void OnClientDisconnected(object sender, EventArgs e) // chiamata quando il client si disconnette
        {
            serverKeyboardHook.Enabled = false;
            serverMouseHook.Enabled = false;

            status = ServerStatus.Listenining;
            RefreshStatusLabel();
            btnStop.Invoke(new MethodInvoker(() => { this.btnStop.Enabled = true; }));
        }

        private void OnReceivingEvents(object sender, EventArgs e)
        {
            serverKeyboardHook.Enabled = true;
            serverMouseHook.Enabled = true;

            status = ServerStatus.ClientActive;
            RefreshStatusLabel();
            btnStop.Invoke(new MethodInvoker(() => { this.btnStop.Enabled = false; }));

            /* qua o quando il client si collega? */
            // consumatore: riproduce gli eventi
            //Task.Factory.StartNew(() =>
            //{
            //    while (connected)
            //        EventsBuffer.ConsumeEvents();
            //});

            Task.Factory.StartNew(() =>
            {
                while (status == ServerStatus.ClientActive)
                    EventsBuffer.ConsumeEvents();
            });

            
        }

        private void OnClientStoppedSendingEvent(object sender, EventArgs e)
        {
            serverKeyboardHook.Enabled = false;
            serverMouseHook.Enabled = false;

            status = ServerStatus.ClientConnected;
            RefreshStatusLabel();
            btnStop.Invoke(new MethodInvoker(() => { this.btnStop.Enabled = true; }));
        }

        private void RefreshStatusLabel()
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new MethodInvoker(() => { this.lblStatus.Text = "Status: " + status.ToString(); }));
                return;
            }
            lblStatus.Text = "Status: " + status.ToString();
        }

    }
}
