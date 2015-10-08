using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace MouseKeyboardLibrary
{

    #region HotKey

    [Serializable]
    public class HotKey
    {
        private List<Keys> sequence;
        private short indexToCheck;
        private string asString;
        public event EventHandler HotKeyHappened;     // evento scatenato quando viene rilevato l'hotkey
        private static KeysConverter kc = new KeysConverter();

        //public HotKey(List<Keys> keys)
        //{
        //    for (int i = 0; i < keys.Count; ++i)
        //        keys[i] = ParseKey(keys[i]);
        //    this.sequence = keys;
        //    indexToCheck = 0;
        //}

        public HotKey(params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; ++i)
                keys[i] = ParseKey(keys[i]);
            sequence = new List<Keys>(keys);
            indexToCheck = 0;

            setString();
        }

        public HotKey(List<KeyEventArgs> keys)
        {
            sequence = new List<Keys>();
            for (int i = 0; i < keys.Count; ++i)
                sequence.Add(ParseKey(keys[i].KeyCode));
            indexToCheck = 0;

            setString();
        }

        #region String Rapresentation

        private void setString()
        {
            string s = "";
            foreach (Keys k in sequence)
                s += (ConvertToString(k) + " + ");
            if (s.Equals(""))
                asString = "";
            else
                asString = s.Substring(0, s.Length - 3);
        }

        private string ConvertToString(Keys k)
        {
            string ret = kc.ConvertToString(k).ToUpper().Replace("KEY", string.Empty).Replace("MENU", "ALT");
            if (ret.Contains("OEM"))
            {
                ret = ret.Replace("OEM", string.Empty);
                switch (ret)
                {
                    case "5": return @"\";
                    case "OPENBRACKETS": return "'";
                    case "6": return "ì";
                    case "1": return "è";
                    case "7": return "à";
                    case "QUESTION": return "ù";
                    case "TILDE": return "ò";
                    case "MINUS": return "-";
                    case "PLUS": return "+";
                    case "PERIOD": return ".";
                    case "COMMA": return ",";
                    case "BACKSLASH": return "<";
                    default: return ret;
                }
            }
            switch (ret)
            {
                case "DIVIDE": return "NUMPAD/";
                case "MULTIPLY": return "NUMPAD*";
                case "SUBTRACT": return "NUMPAD-";
                case "ADD": return "NUMPAD+";
                case "DECIMAL": return "NUMPAD.";
            }
            return ret;
        }

        public override string ToString()
        {
            return asString;
        }

        #endregion

        public void CheckHotKey(KeyEventArgs key)
        {
            if (sequence == null || sequence.Count == 0)
                return;

            if (sequence[indexToCheck].Equals(ParseKey(key.KeyCode)))
            {
                indexToCheck++;
                if (indexToCheck == sequence.Count)
                {
                    indexToCheck = 0;
                    OnHotKeyHappened(EventArgs.Empty);
                }
            }
            else
                indexToCheck = 0;
        }

        public void OnHotKeyHappened(EventArgs e)
        {
            if (HotKeyHappened != null)
                HotKeyHappened(this, e);
        }

        public void ResetCheckCounter()
        {
            indexToCheck = 0;
        }

        public int Size()
        {
            return sequence.Count;
        }

        public static Keys ParseKey(Keys k)
        {
            switch (k)
            {
                case Keys.LControlKey:
                case Keys.RControlKey: return Keys.ControlKey;
                case Keys.LShiftKey:
                case Keys.RShiftKey: return Keys.ShiftKey;
                case Keys.LMenu:
                case Keys.RMenu: return Keys.Menu;
                default: return k;
            }
        }

        public void SimulateKeysUp()
        {
            foreach (Keys k in sequence)
            {
                switch (k)
                {
                    case Keys.ControlKey: KeyboardSimulator.KeyUp(Keys.LControlKey);
                        KeyboardSimulator.KeyUp(Keys.RControlKey);
                        break;
                    case Keys.ShiftKey: KeyboardSimulator.KeyUp(Keys.LShiftKey);
                        KeyboardSimulator.KeyUp(Keys.RShiftKey);
                        break;
                    case Keys.Menu: KeyboardSimulator.KeyUp(Keys.LMenu);
                        KeyboardSimulator.KeyUp(Keys.RMenu);
                        break;
                    default: KeyboardSimulator.KeyUp(k);
                        break;
                }
            }
        }

        public bool HasSameKeysSequence(HotKey hk)
        {   
            int minSize = Size() < hk.Size() ? Size() : hk.Size();
            for (int i = 0; i < minSize; ++i)
                if (sequence[i] != hk.sequence[i])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is HotKey))
                return false;

            return Equals((HotKey)obj);
        }

        public bool Equals(HotKey hk)
        {
            if (hk == null || Size() != hk.Size())
                return false;

            for (int i = 0; i < Size(); ++i)
                if (sequence[i] != hk.sequence[i])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hc = 0;
            foreach (Keys k in sequence)
                hc ^= (int)k;
            return hc;
        }
    }

    #endregion

    #region HotKeyRecorder

    public class HotKeyRecordedArgs : EventArgs
    {
        public HotKey hotKey;
    }

    //public delegate string KeyRecorded();

    public class HotKeyRecorder
    {
        private static readonly int minKeys = 2;
        private ClientKeyboardHook kbHook;
        private List<KeyEventArgs> buffer;
        private bool _recording;
        public event EventHandler<HotKeyRecordedArgs> HotKeyRecorded, KeyRecorded;
        //public event EventHandler< KeyRecorded;

        public bool Recording { get { return _recording; } private set { _recording = value; } }

        public HotKeyRecorder()
        {
            kbHook = new ClientKeyboardHook();
            kbHook.KeyDown += KeyDownHandler;
            kbHook.KeyUp += KeyUpHandler;
        }

        public void Rec()
        {
            if (_recording) return;
            buffer = new List<KeyEventArgs>();
            _recording = true;
            kbHook.Start();
        }

        public void Stop()
        {
            if (!_recording) return;
            _recording = false;
            kbHook.Stop();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (_recording && (buffer.Count == 0 || !HotKey.ParseKey(e.KeyCode).Equals(HotKey.ParseKey(buffer[buffer.Count - 1].KeyCode))))
            {
                buffer.Add(e);
                OnKeyRecorded(new HotKeyRecordedArgs() { hotKey = new HotKey(buffer) });
            }
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (_recording && buffer.Count >= minKeys)  // gestire duplicazione hotkey?
                OnHotKeyRecorded(new HotKeyRecordedArgs() { hotKey = new HotKey(buffer) });
            buffer = new List<KeyEventArgs>();
        }

        protected void OnHotKeyRecorded(HotKeyRecordedArgs e)
        {
            if (HotKeyRecorded != null)
            {
                HotKeyRecorded(this, e);
            }
        }
        protected void OnKeyRecorded(HotKeyRecordedArgs e)
        {
            if (KeyRecorded != null)
            {
                KeyRecorded(this, e);
            }
        }

    }

    #endregion

    #region HotKeyListener

    public class HotKeyListener
    {
        private List<HotKey> hotKeys;

        public HotKeyListener()
        {
            this.hotKeys = new List<HotKey>();
        }
        public HotKeyListener(List<HotKey> hotKeys)
        {
            this.hotKeys = hotKeys;
        }

        public bool Add(HotKey hk)  // gestire duplicazione hotkey?
        {
            foreach (HotKey hkTmp in hotKeys)
                if (hk.HasSameKeysSequence(hkTmp))
                    return false;
            hotKeys.Add(hk);
            return true;
        }

        public bool Remove(HotKey hk)
        {
            return hotKeys.Remove(hk);
        }

        public void KeyDownListen(object sender, KeyEventArgs e)
        {
            foreach (HotKey hk in hotKeys)
                hk.CheckHotKey(e);
        }

        public void KeyUpListen(object sender, KeyEventArgs e)
        {
            foreach (HotKey hk in hotKeys)
                hk.ResetCheckCounter();
        }
    }

    #endregion

}
