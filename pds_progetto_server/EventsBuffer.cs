using System;
using System.Collections.Generic;
using MouseKeyboardLibrary;
using System.Threading;
using System.Drawing;

namespace pds_progetto_server
{
    public static class EventsBuffer
    {
        private static List<MacroEvent> list = new List<MacroEvent>();
        private static Object lockObj = new Object();

        public static void AddEvents(List<MacroEvent> l)
        {
            lock (lockObj)
            {
                list.AddRange(l);
                Monitor.Pulse(lockObj);
            }            
        }

        public static void ConsumeEvents()
        {
            lock (lockObj)
            {
                while (list.Count == 0)
                    Monitor.Wait(lockObj);

                playEvents(list);
                list.Clear();
            }
        }

        //public static void ConsumeEvents()
        //{
        //    List<MacroEvent> tmpList;
        //    lock (list)
        //    {
        //        while (list.Count == 0)
        //            Monitor.Wait(list);

        //        tmpList = list;
        //        list = new List<MacroEvent>();
        //    }
        //    //return ret;
        //    playEvents(tmpList);
        //}

        private static void playEvents(List<MacroEvent> events)
        {
            foreach (MacroEvent macroEvent in events)
            {
                //Thread.Sleep()
                switch (macroEvent.MacroEventType)
                {
                    case MacroEventType.MouseMove:
                        {
                            MacroMouseEventArgs mouseArgs = macroEvent.EventArgs.mouse;
                            //MouseSimulator.Position = new Point(mouseArgs.X, mouseArgs.Y);
                            MouseSimulator.setPositionFromNormalizedDelta(mouseArgs.X, mouseArgs.Y);
                        }
                        break;
                    case MacroEventType.MouseDown:
                        {
                            MacroMouseEventArgs mouseArgs = macroEvent.EventArgs.mouse;
                            MouseSimulator.MouseDown(mouseArgs.Button);
                        }
                        break;
                    case MacroEventType.MouseUp:
                        {
                            MacroMouseEventArgs mouseArgs = macroEvent.EventArgs.mouse;
                            MouseSimulator.MouseUp(mouseArgs.Button);
                        }
                        break;
                    case MacroEventType.MouseWheel:
                        {
                            MacroMouseEventArgs mouseArgs = macroEvent.EventArgs.mouse;
                            MouseSimulator.MouseWheel(mouseArgs.Delta);
                        }
                        break;
                    case MacroEventType.KeyDown:
                        {
                            MacroKeyEventArgs keyArgs = macroEvent.EventArgs.key;
                            KeyboardSimulator.KeyDown(keyArgs.KeyCode);
                        }
                        break;
                    case MacroEventType.KeyUp:
                        {
                            MacroKeyEventArgs keyArgs = macroEvent.EventArgs.key;
                            KeyboardSimulator.KeyUp(keyArgs.KeyCode);
                        }
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
