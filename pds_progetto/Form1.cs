using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using MouseKeyboardLibrary;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/* CLIENT */

namespace pds_progetto
{
    public partial class Form1 : Form
    {
        //CONFIGURATION
        static Server_farm Server_farm = new Server_farm();


        volatile bool connected = false;
        volatile bool active = false;
        private ClientSocket cs = new ClientSocket();

        private List<MacroEvent> events = new List<MacroEvent>();       // buffer su cui vengono gli eventi fra un tick e l'altro
        private ClientMouseHook mouseHook = new ClientMouseHook();      // associa degli handler degli eventi mouse, senza eseguire la CallNextHookEx
        private ClientKeyboardHook keyboardHook = new ClientKeyboardHook();     // associa degli handler degli eventi keyboard, senza eseguire la CallNextHookEx
        private HotKeyListener hkListener = new HotKeyListener();       // ascolta se viene premuto una combinazione di tasti e ne esegue l'handler

        public Form1()
        {
            InitializeComponent();

            HotKey hk = new HotKey(Keys.LControlKey, Keys.F12);
            // Evento eseguito quando viene premuto l'hotkey: smetti di catturare e inviare gli eventi (flag active da true a false)
            hk.HotKeyHappened += (object sender, EventArgs e) =>
                {
                    //simulare keyup qua
                    hk.SimulateKeysUp();

                    keyboardHook.Stop();
                    mouseHook.Stop();
                    //Thread.Sleep(timer1.Interval);
                    timer1.Stop();

                    if (events.Count > 0)
                    {
                        EventsBuffer.AddEvents(events);
                        cs.SendEventsList(EventsBuffer.ConsumeEvents());
                        events = new List<MacroEvent>();
                    }

                    active = false;
                    EventsBuffer.Close();
                };
            hkListener.Add(hk);

            //foreach(server s in serverfarm)
            // s.hotkey.hotkeyhappened += clientsocket.connecttoserver(s.ip,s.port);

            mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);
            mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);
            mouseHook.MouseWheel += new MouseEventHandler(mouseHook_MouseWheel);

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);
            // Handlers di HotKeyListener vengono eseguiti dopo che gli eventi vengono aggiunti alla lista da inviare
            keyboardHook.KeyDown += hkListener.KeyDownListen;
            keyboardHook.KeyUp += hkListener.KeyUpListen;


            // Evento: ClientSocket si collega al server (connected da false a true)
            cs.Connected += (object sender, EventArgs e) =>
            {
                connected = true;
                btnAttiva.Invoke(new MethodInvoker(() => { this.btnAttiva.Enabled = true; }));
                btnConnetti.Invoke(new MethodInvoker(() => { this.btnConnetti.Text = "Disconnetti"; }));
            };

            // Evento: ClientSocker non è più collegato al server (connected da true a false)
            cs.Disconnected += (object sender, EventArgs e) =>
            {
                connected = false;
                btnAttiva.Invoke(new MethodInvoker(() => { this.btnAttiva.Enabled = false; }));
                btnConnetti.Invoke(new MethodInvoker(() => { this.btnConnetti.Text = "Connetti"; }));
                
                // se il server si disconnette mentre il client è attivo
                if (active)
                {
                    keyboardHook.Stop();
                    mouseHook.Stop();
                    timer1.Stop();
                    active = false;
                }
                //EventsBuffer.Close();
            };

            loadServerFarm();

            foreach (Server s in Server_farm.server_list.Values)
                listView1.Items.Add(s.Name);

            //lv.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            //listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.Columns[0].Width = listView1.Width - 4;

        }

        private void btnStart_Click(object sender, EventArgs e)     //btnConnetti: per connettersi/disconnettersi dal server
        {
            if (connected)
            {
                cs.Close();
                return;
            }          

            cs.ConnectToServer(this.txtIpAddress.Text, Int32.Parse(this.txtPortNumber.Text));      
        }

        private void btnStop_Click(object sender, EventArgs e)      //btnAttiva: per iniziare ad inviare eventi al server (per finire usare hotkey)
        {
            if (!connected) return;

            active = true;
            events.Clear();
            EventsBuffer.Clear();

            keyboardHook.Start();
            mouseHook.Start();
            timer1.Start();

            Task.Factory.StartNew(() =>
            {
                cs.SendInt(0);
                try
                {
                    cs.SendClipBoard();

                    cs.SendLockedKeysStatus();
                    while (true)
                        cs.SendEventsList(EventsBuffer.ConsumeEvents());
                }
                catch (EventsBufferClosedException)
                {
                    //Console.
                    cs.SendInt(-1);
                    cs.recClipBoard();
                    //active = false;
                    //cs deve ricevere la clipboard dal server
                }
            });

        }

        private void timer1_Tick(object sender, EventArgs e)        //tick: invia gli eventi raccolti
        {
            if (events.Count == 0)
                return;
            //Console.WriteLine("Tick: list containing {0} elements", events.Count);
            EventsBuffer.AddEvents(events);
            events = new List<MacroEvent>();
        }

        #region Handlers

        void mouseHook_MouseWheel(object sender, MouseEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.MouseWheel,
                    e,
                    0
                ));
        }

        void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
                events.Add(
                    new MacroEvent(
                        MacroEventType.MouseMove,
                        e,
                        0
                    ));
        }

        void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.MouseDown,
                    e,
                    0
                ));
        }

        void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.MouseUp,
                    e,
                    0
                ));
        }

        void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.KeyDown,
                    e,
                    0
                ));
        }

        void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.KeyUp,
                    e,
                    0
                ));
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Write to file
        private static void storeServerFarm()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter br = new BinaryFormatter();
            using (FileStream fs = new FileStream("server_farmNOSTATIC.dat", FileMode.OpenOrCreate))
            {
                try
                {
                    br.Serialize(ms, Server_farm);
                    ms.WriteTo(fs);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        //Read from file
        private static void loadServerFarm()
        {
            try
            {
                using (FileStream fs = new FileStream("server_farmNOSTATIC.dat", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    fs.Seek(0, SeekOrigin.Begin);
                    Server_farm = (bf.Deserialize(fs) as Server_farm);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Il file di configurazione non è stato trovato.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

        }

        
        //Initialize details view 
        private void btn_addNewServer_Click(object sender, EventArgs e)
        {
            txtIpAddress.Text = "";
            txtPortNumber.Text = "";
            txtPassword.Text = "";
            txtHotKey.Text = "";
            txtName.Text = "";

        }

        //Save New Server or Update Existing Server
        private void btn_saveServer_Click(object sender, EventArgs e)
        {
            if (validArgumentsForServer(txtName.Text, txtPassword.Text, null, txtIpAddress.Text, txtPortNumber.Text))
            {

                Server s = new Server(txtName.Text, txtPassword.Text, null, IPAddress.Parse(txtIpAddress.Text), int.Parse(txtPortNumber.Text));

                if (listView1.SelectedItems.Count == 0)
                {
                    Server_farm.saveNewServer(s);
                    listView1.Items.Add(s.Name);
                }
                else
                {
                    if (Server_farm.server_list.ContainsKey(txtName.Text))
                        Server_farm.updateServer(s);
                    else
                    {
                        Server_farm.replaceServer(listView1.SelectedItems[0].Text, s);
                        listView1.SelectedItems[0].Text = s.Name;
                    }
                }

                //Resolve eventually problem for sizing column
                //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            }

            Console.WriteLine("Server_farm count: {0}", Server_farm.server_list.Count);

        }

        //The rigth way to build a Server. Call this before pass argument to constructor.
        public bool validArgumentsForServer(String name, String password, Object hk, String ip, String port)
        {
            bool _name = false, _password = false, _hk = false, _ip = false, _port = false;

            //VISUALIZZARE A VIDEO L'INFORMAZIONE ERRATA

            // 1. NAME
            if (name.Length > 0)
            {
                _name = true;
                txtName.ForeColor = DefaultForeColor;
            }
            else
                txtName.ForeColor = System.Drawing.Color.Red;

            // 2. PASSWORD
            if (password.Length > 4)
            {
                _password = true;
                txtPassword.ForeColor = DefaultForeColor;
            }
            else
                txtPassword.ForeColor = System.Drawing.Color.Red;

            // 3. HOT KEY
            if (hk == null)
            {
                //ne assegnamo uno di default
                _hk = true;
            }
            else
            {
                bool found = false;
                foreach (Object hk_i in Server_farm.getHotKeys())
                {
                    if (hk_i.Equals(hk))
                        found = true;
                }

                if (found == false)
                    _hk = true;

            }

            // 4. IP
            try
            {
                IPAddress.Parse(ip);
                _ip = true;
                txtIpAddress.ForeColor = DefaultForeColor;
            }
            catch (FormatException fe)
            {
                txtIpAddress.ForeColor = System.Drawing.Color.Red;
                Console.WriteLine("Indirizzo IP non valido");
            }

            // 5. PORTA
            try
            {

                int int_port = int.Parse(port);
                if (int_port > 1000 && int_port < 20000)
                {
                    _port = true;
                    txtPortNumber.ForeColor = DefaultForeColor;
                }
                else txtPortNumber.ForeColor = System.Drawing.Color.Red;

            }
            catch (FormatException fe)
            {
                txtPortNumber.ForeColor = System.Drawing.Color.Red;
                Console.WriteLine("Valore porta non valido");
            }

            //TUTTI CAMPI VALIDI?
            if (_name && _password && _hk && _ip && _port)
                return true;

            return false;
        }

        //Initialize details View with Info of a Server
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;


            ListViewItem item = listView1.SelectedItems[0];
            Console.WriteLine("[CLICK ON LISTVIEW] Element selected: {0}", item.Text);

            //Reload Details views - Binding!
            if (Server_farm.server_list.ContainsKey(item.Text))
            {
                Server s = Server_farm.server_list[item.Text];
                txtIpAddress.Text = s.Ip.ToString();
                txtPortNumber.Text = s.Port.ToString();
                txtPassword.Text = s.Password;
                //txtHotKey.Text     = s.HotKey.ToString();
                txtName.Text = s.Name;
            }
            else
                Console.WriteLine("[CLICK ON LISTVIEW] POBBBBBBLEEEEMIII");
        }

        //Prevents column width change
        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = this.listView1.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        //RightClick on a Server - to delete
        private void listView1_ItemMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count == 0)
                    return;

                this.contextMenuStrip1.Show(this.listView1, e.Location);

            }
        }

        //Delete an existing Server
        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            ListViewItem item = listView1.SelectedItems[0];
            String name_server = item.Text;

            this.listView1.Items.Remove(item);
            Server_farm.server_list.Remove(name_server);

            //Force resizing column
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        //Register new HotKey for a Server
        private void textBoxHotKey_TextChanged(object sender, EventArgs e)
        {

            Console.WriteLine("[REPORT BUTTON HOTKEY] - Enable HOTKEY_LISTENER and set Server named: {0}", txtName.Text);

        }

        //Restore default text color in a textBox that had a errors
        private void textBoxDetails_EnterFocus(object sender, EventArgs e)
        {
            TextBox txt = ((TextBox)sender);
            txt.ForeColor = DefaultForeColor;

        }

        //Allow to set a new HOTKEY for exit
        private void label_EXIT_HotKey_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Active listener to capture a new HotKey_EXIT");
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            //UGUALE AL TASTO ATTIVA
            if (!connected) return;

            active = true;
            events.Clear();
            EventsBuffer.Clear();

            keyboardHook.Start();
            mouseHook.Start();
            timer1.Start();

            Task.Factory.StartNew(() =>
            {
                cs.SendInt(0);
                try
                {
                    cs.SendClipBoard();

                    cs.SendLockedKeysStatus();
                    while (true)
                        cs.SendEventsList(EventsBuffer.ConsumeEvents());
                }
                catch (EventsBufferClosedException)
                {
                    //Console.
                    cs.SendInt(-1);
                   
                    //active = false;
                    //cs deve ricevere la clipboard dal server
                }
            });
        }

        private void txtIpAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnStop_Click_1(object sender, EventArgs e)
        {
            storeServerFarm();
        }
    }
}
