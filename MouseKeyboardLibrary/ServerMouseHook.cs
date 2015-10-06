﻿using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace MouseKeyboardLibrary
{
  
    /// <summary>
    /// Captures global mouse events
    /// </summary>
    public class ServerMouseHook : GlobalHook
    {
        public volatile bool Enabled;

        #region MouseEventType Enum

        private enum MouseEventType
        {
            None,
            MouseDown,
            MouseUp,
            DoubleClick,
            MouseWheel,
            MouseMove
        }

        #endregion

        #region Events

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;

        public event EventHandler Click;
        public event EventHandler DoubleClick;

        #endregion

        #region Constructor

        public ServerMouseHook()
        {

            _hookType = WH_MOUSE_LL;

        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            if (Enabled && nCode > -1)
            {

                MouseLLHookStruct mouseHookStruct =
                    (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                //Console.WriteLine("X:{0}\tY:{1}\tFlag:{2}", mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseHookStruct.flags);
                if (mouseHookStruct.flags == 0x00000000) //evento reale -> scarta
                    return 1;
                
                MouseButtons button = GetButton(wParam);
                MouseEventType eventType = GetEventType(wParam);

                MouseEventArgs e = new MouseEventArgs(
                    button,
                    (eventType == MouseEventType.DoubleClick ? 2 : 1),
                    mouseHookStruct.pt.x,
                    mouseHookStruct.pt.y,
                    
                    (eventType == MouseEventType.MouseWheel ? (short)((mouseHookStruct.mouseData >> 16) & 0xffff) : 0));

                // Prevent multiple Right Click events (this probably happens for popup menus)
                if (button == MouseButtons.Right && mouseHookStruct.flags != 0)
                {
                    eventType = MouseEventType.None;
                }

               

                switch (eventType)
                {
                    case MouseEventType.MouseDown:
                        if (MouseDown != null)
                        {
                            MouseDown(this, e);
                        }
                        //return 1;
                        break;
                    case MouseEventType.MouseUp:
                        if (Click != null)
                        {
                            Click(this, new EventArgs());
                        }
                        if (MouseUp != null)
                        {
                            MouseUp(this, e);
                        }
                        break;
                    case MouseEventType.DoubleClick:
                        if (DoubleClick != null)
                        {
                            DoubleClick(this, new EventArgs());
                        }
                        break;
                    case MouseEventType.MouseWheel:
                        if (MouseWheel != null)
                        {
                            MouseWheel(this, e);
                        }
                        break;
                    case MouseEventType.MouseMove:
                        if (MouseMove != null)
                        {
                            MouseMove(this, e);
                        }
                        //return 1;
                        break;
                    default:
                        break;
                }
                
            }

            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
            //return 1;
        }

        private MouseButtons GetButton(Int32 wParam)
        {

            switch (wParam)
            {

                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                    return MouseButtons.Left;
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                    return MouseButtons.Right;
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                    return MouseButtons.Middle;
                default:
                    return MouseButtons.None;

            }

        }

        private MouseEventType GetEventType(Int32 wParam)
        {

            switch (wParam)
            {

                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                    return MouseEventType.MouseDown;
                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                case WM_MBUTTONUP:
                    return MouseEventType.MouseUp;
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDBLCLK:
                    return MouseEventType.DoubleClick;
                case WM_MOUSEWHEEL:
                    return MouseEventType.MouseWheel;
                case WM_MOUSEMOVE:
                    return MouseEventType.MouseMove;
                default:
                    return MouseEventType.None;

            }
        }

        #endregion



    }



}