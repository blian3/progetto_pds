using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    
    /// <summary>
    /// Mouse buttons that can be pressed
    /// </summary>
    public enum MouseButton
    {
        Left = 0x2,
        Right = 0x8,
        Middle = 0x20
    }

    /// <summary>
    /// Operations that simulate mouse events
    /// </summary>
    public static class MouseSimulator
    {
        //private static double[] scaleValue = new double[] { 0, 0 };
        //private static int max_x, max_y;
        private static readonly int NORMALIZE_FACTOR = 65536;

        #region Windows API Code

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool show);

        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        const int MOUSEEVENTF_MOVE = 0x1;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a structure that represents both X and Y mouse coordinates
        /// </summary>
        public static Point Position
        {
            get
            {
                return new Point(Cursor.Position.X, Cursor.Position.Y);
            }
            set
            {
                //MOD
                //Cursor.Position = scalePoint(value);
                Cursor.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets only the mouse's x coordinate
        /// </summary>
        public static int X
        {
            get
            {
                return Cursor.Position.X;
            }
            set
            {
                //MOD
               //Cursor.Position = new Point((int)(value * scaleValue[0]), Y);
                Cursor.Position = new Point(value, Y);
            }
        }

        /// <summary>
        /// Gets or sets only the mouse's y coordinate
        /// </summary>
        public static int Y
        {
            get
            {
                return Cursor.Position.Y;
            }
            set
            {
                //MOD
                //Cursor.Position = new Point(X, (int)(value * scaleValue[1]));
                Cursor.Position = new Point(X, value);
            }
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Press a mouse button down
        /// </summary>
        /// <param name="button"></param>
        public static void MouseDown(MouseButton button)
        {
            mouse_event(((int)button), 0, 0, 0, 0);
        }

        /// <summary>
        /// Press a mouse button down
        /// </summary>
        /// <param name="button"></param>
        public static void MouseDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    MouseDown(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    MouseDown(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    MouseDown(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// Let a mouse button up
        /// </summary>
        /// <param name="button"></param>
        public static void MouseUp(MouseButton button)
        {
            mouse_event(((int)button) * 2, 0, 0, 0, 0);
        }

        /// <summary>
        /// Let a mouse button up
        /// </summary>
        /// <param name="button"></param>
        public static void MouseUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    MouseUp(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    MouseUp(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    MouseUp(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// Click a mouse button (down then up)
        /// </summary>
        /// <param name="button"></param>
        public static void Click(MouseButton button)
        {
            MouseDown(button);
            MouseUp(button);
        }

        /// <summary>
        /// Click a mouse button (down then up)
        /// </summary>
        /// <param name="button"></param>
        public static void Click(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    Click(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    Click(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    Click(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// Double click a mouse button (down then up twice)
        /// </summary>
        /// <param name="button"></param>
        public static void DoubleClick(MouseButton button)
        {
            Click(button);
            Click(button);
        }

        /// <summary>
        /// Double click a mouse button (down then up twice)
        /// </summary>
        /// <param name="button"></param>
        public static void DoubleClick(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    DoubleClick(MouseButton.Left);
                    break;
                case MouseButtons.Middle:
                    DoubleClick(MouseButton.Middle);
                    break;
                case MouseButtons.Right:
                    DoubleClick(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// Roll the mouse wheel. Delta of 120 wheels up once normally, -120 wheels down once normally
        /// </summary>
        /// <param name="delta"></param>
        public static void MouseWheel(int delta)
        {

            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, delta, 0);

        }

        /// <summary>
        /// Show a hidden current on currently application
        /// </summary>
        public static void Show()
        {
            ShowCursor(true);
        }

        /// <summary>
        /// Hide mouse cursor only on current application's forms
        /// </summary>
        public static void Hide()
        {
            ShowCursor(false);
        }

        #endregion





        /*  MOD
            Resize Point
        */
        //static public Point scalePoint(Point point)
        //{
        //    int tmpX = (int)(scaleValue[0] * point.X);
        //    int tmpY = (int)(scaleValue[1] * point.Y);
        //    point.X = tmpX > max_x ? max_x : (tmpX < 0 ? 0 : tmpX);
        //    point.Y = tmpY > max_y ? max_y : (tmpY < 0 ? 0 : tmpY);
        //    //Console.WriteLine("X:{0}\tY:{1}\tSimulator", point.X, point.Y);
        //    return point;
        //}


        static public void setPositionFromNormalizedDelta(int dx, int dy)
        {
            int tmpX = Cursor.Position.X + (dx * Screen.PrimaryScreen.Bounds.Width / NORMALIZE_FACTOR);
            int tmpY = Cursor.Position.Y + (dy * Screen.PrimaryScreen.Bounds.Height / NORMALIZE_FACTOR);
            Position = new Point(tmpX > Screen.PrimaryScreen.Bounds.Width ? Screen.PrimaryScreen.Bounds.Width : (tmpX < 0 ? 0 : tmpX), tmpY > Screen.PrimaryScreen.Bounds.Height ? Screen.PrimaryScreen.Bounds.Height : (tmpY < 0 ? 0 : tmpY));
            //Console.WriteLine("dx:{0} \t dy:{1} \t X:{2} \tY:{3}", dx, dy, Position.X, Position.Y);
        }

        /*  MOD
            Find two scale factor, one for each coordinate.
            Value are very similar.          
        */
        //static public void findScaleValue(double client_h, double client_w) {
        //    //Console.WriteLine("Resolution received H: {0}  W:{1}", client_h, client_w);
        //    //Rectangle resolution = Screen.PrimaryScreen.Bounds;
        //    max_x = Screen.PrimaryScreen.Bounds.Width;
        //    max_y = Screen.PrimaryScreen.Bounds.Height;
        //    double _x = 1, _y = 1;
        //    if (client_h != max_y || client_w != max_x)
        //    {
        //        _x = max_x / client_w;
        //        _y = max_y / client_h;       
        //    }
        //    scaleValue[0] = _x;
        //    scaleValue[1] = _y;
        //}
    }

}
