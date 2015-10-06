using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    [Serializable]
    public enum MacroEventType
    {
        MouseMove,
        MouseDown,
        MouseUp,
        MouseWheel,
        KeyDown,
        KeyUp
    }

    /// <summary>
    /// Series of events that can be recorded any played back
    /// </summary>
    [Serializable]
    public class MacroEvent
    {

        public MacroEventType MacroEventType;
        public MacroEventArgs EventArgs;
        public int TimeSinceLastEvent;

        public MacroEvent(MacroEventType macroEventType, EventArgs eventArgs, int timeSinceLastEvent)
        {

            MacroEventType = macroEventType;
            EventArgs = new MacroEventArgs(eventArgs);
            TimeSinceLastEvent = timeSinceLastEvent;

        }

    }




    [Serializable]
    public class MacroEventArgs : EventArgs
    {

        public MacroMouseEventArgs mouse = null;
        public MacroKeyEventArgs key = null;

        public MacroEventArgs(EventArgs e)
        {
            if (e.GetType() == typeof(MouseEventArgs))
            {
                mouse = new MacroMouseEventArgs((MouseEventArgs)e);
            }
            if (e.GetType() == typeof(KeyEventArgs))
            {
                key = new MacroKeyEventArgs((KeyEventArgs)e);
            }
        }

    }

    [Serializable]
    public class MacroMouseEventArgs
    {
        public MacroMouseEventArgs(MouseEventArgs e)
        {
            this.Button = e.Button;
            this.Clicks = e.Clicks;
            this.X = e.X;
            this.Y = e.Y;
            this.Delta = e.Delta;
        }

        /*public MacroMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) {
            this.Button = button;
            this.Clicks = clicks;
            this.X = x;
            this.Y = y;
            this.Delta = delta;
    }*/

        public MouseButtons Button { get; private set; }
        public int Clicks { get; private set; }
        public int Delta { get; private set; }
        public Point Location { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

    }

    [Serializable]
    public class MacroKeyEventArgs
    {
        public MacroKeyEventArgs(KeyEventArgs e)
        {
            this.Alt = e.Alt;
            this.Control = e.Control;
            this.Handled = e.Handled;
            this.KeyCode = e.KeyCode;
            this.KeyData = e.KeyData;
            this.KeyValue = e.KeyValue;
            this.Modifiers = e.Modifiers;
            this.Shift = e.Shift;
            this.SuppressKeyPress = e.SuppressKeyPress;
        }

        public virtual bool Alt { get; private set; }
        public bool Control { get; private set; }
        public bool Handled { get; set; }
        public Keys KeyCode { get; private set; }
        public Keys KeyData { get; private set; }
        public int KeyValue { get; private set; }
        public Keys Modifiers { get; private set; }
        public virtual bool Shift { get; private set; }
        public bool SuppressKeyPress { get; set; }

    }
}
