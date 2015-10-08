using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    [Serializable]
    public class ConfigureStatus
    {
        public bool CapsLocked, NumLocked;
        public bool SendClip, RecClip;

        public ConfigureStatus(bool send, bool rec)
        {
            CapsLocked = Control.IsKeyLocked(Keys.CapsLock);
            NumLocked = Control.IsKeyLocked(Keys.NumLock);
            SendClip = send;
            RecClip = rec;
        }
    }
}
