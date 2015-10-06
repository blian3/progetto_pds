using MouseKeyboardLibrary;
using System;
using System.Collections.Generic;
using System.Threading;

namespace pds_progetto
{
    class EventsBuffer
    {
        private static List<MacroEvent> list = new List<MacroEvent>();
        private static volatile bool closed = false;
        private static Object lockObj = new Object();

        public static void AddEvents(List<MacroEvent> l)
        {
            lock (lockObj)
            {
                if (closed)
                    return;

                list.AddRange(l);
                Monitor.Pulse(lockObj);
            }
        }

        public static List<MacroEvent> ConsumeEvents()
        {
            List<MacroEvent> tmp;
            lock (lockObj)
            {
                while (list.Count == 0 && !closed)
                    Monitor.Wait(lockObj);

                if (closed)
                    throw new EventsBufferClosedException();

                tmp = list;
                list = new List<MacroEvent>();

                //foreach (MacroEvent e in tmp)
                //{
                //    if (e.MacroEventType == MacroEventType.KeyUp)
                //        Console.WriteLine("Removed from EventsBuffer: {0}", e.EventArgs.key.KeyCode.ToString());
                //}
            }

            return tmp;
        }

        public static void Clear()
        {
            lock (lockObj)
            {
                list.Clear();
                closed = false;
            }
        }

        public static void Close()
        {
            lock (lockObj)
            {
                closed = true;
                Monitor.Pulse(lockObj);
            }
        }
    }
}
