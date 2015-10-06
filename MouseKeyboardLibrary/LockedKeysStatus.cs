using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    [Serializable]
    public class LockedKeysStatus
    {
        public bool CapsLocked, NumLocked;
        public LockedKeysStatus()
        {
            CapsLocked = Control.IsKeyLocked(Keys.CapsLock);
            NumLocked = Control.IsKeyLocked(Keys.NumLock);
        }
    }
}
