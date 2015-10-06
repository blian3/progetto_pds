using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseKeyboardLibrary;

namespace HotKeyTest
{
    public partial class Form1 : Form
    {
        static HotKeyRecorder hkRecorder;

        static ClientKeyboardHook kbHook;
        static HotKeyListener hkListener;
        public Form1()
        {
            InitializeComponent();
            hkRecorder = new HotKeyRecorder();
            hkListener = new HotKeyListener();
            kbHook = new ClientKeyboardHook();
            kbHook.KeyDown += hkListener.KeyDownListen;
            kbHook.KeyUp += hkListener.KeyUpListen;

            hkRecorder.HotKeyRecordedEvent += (HotKey hk) =>
            {
                hk.HotKeyHappened += hk_HotKeyEvent;
                hkListener.Add(hk);
                label1.Text += "HotKeyRegistered: " + hk.ToString() + "\n";
            };
        }

        void hk_HotKeyEvent(object sender, EventArgs e)
        {
            label1.Text += "HotKeyListened: " + ((HotKey)sender).ToString() + "\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hkRecorder.Rec();
            //Task.Factory.StartNew(() => {
            //    label1.Text = "HotKey: " + hkRecorder.Rec().ToString();
            //});
        }

        private void button2_Click(object sender, EventArgs e)
        {
            hkRecorder.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            kbHook.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            kbHook.Stop();
        }
    }
}
