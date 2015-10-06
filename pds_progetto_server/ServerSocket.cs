using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

using MouseKeyboardLibrary;
using System.Collections.Generic;
using System.Windows.Forms;

public delegate void SocketClosedHandler();

namespace pds_progetto_server
{
    public class ServerSocket
    {
        private BinaryFormatter bf = new BinaryFormatter();
        private byte[] buf_int = new byte[sizeof(int)];
        private byte[] buf_eventsList;

        Socket handler;
        public event EventHandler ClientConnected, ClientDisconnected;
        public event EventHandler ClientSendEventsStarted, ClientSendEventsEnded;
        //bool firstEventReceveid;

        private IPEndPoint localEndPoint;
        private Socket listener;

        public void StartListening(int port)
        {
            localEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(1);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");

                    try
                    {
                        handler = listener.Accept();
                        Console.WriteLine("{0} connected...", handler.RemoteEndPoint.ToString());
                        OnClientConnected(EventArgs.Empty);
                        /* Ricezione password, risoluzione, clipboard... */

                        while (true)
                        {
                            int tmp = recInt();
                            if (tmp != 0) return;

                            OnClientSendEventsStarted(EventArgs.Empty);
                            try
                            {
                                recClipBoard();
                                recLockedKeysStatus();

                                while (true)
                                    recEventsListLoop();
                            }
                            catch (ClientStoppedSendingEventsException)
                            {
                                // invio clipboard al client
                                SendClipBoard();
                                OnClientSendEventsEnded(EventArgs.Empty);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.ToString());
                        Close();
                        if (listener == null)       // è stata chiamata la stoplistenening
                            return;                 // quindi il thread non deve tornare a bloccarsi sulla Accept()
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Bind() or Listen() failed\n" + e.ToString());
                listener.Close();
                //listener = null;
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }



        public void StopListening()
        {
            if (listener != null)
                listener.Close();
            listener = null;
        }

        public void Close()
        {
            if (handler != null)
            {
                bool wasConnected = handler.Connected;
                try
                {
                    handler.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    handler.Close();
                    handler = null;
                    //if (wasConnected)
                    OnClientDisconnected(EventArgs.Empty);
                }
            }
        }

        private int handledReceive(byte[] buffer, int size)
        {
            return handledReceive(buffer, 0, size);
        }

        private int handledReceive(byte[] buffer, int offset, int size)
        {
            int ret = handler.Receive(buffer, offset, size, SocketFlags.None);
            // receive ritorna 0 se il socket viene chiuso mentre è in attesa
            // se il socket viene chiuso prima della receive => SocketException
            if (ret == 0)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                handler = null;
                OnClientDisconnected(EventArgs.Empty);
                throw new Exception("Receive returned 0 -> connection closed by client.");
            }
            return ret;
        }

        //private List<MacroEvent> recEventsList()
        //{
        //    int recvBytes = 0;
        //    int totRecvBytes = 0;

        //    int dataSize = recInt();
        //    //Console.WriteLine("Received data size {0}", dataSize);

        //    if (dataSize < 0)
        //    {
        //        if (firstEventReceveid)
        //            OnClientSendEventsEnded(EventArgs.Empty);
        //        firstEventReceveid = false;
        //        Console.WriteLine("Received DataSize {0}", dataSize);
        //        return null;
        //    }

        //    if (!firstEventReceveid)
        //    {
        //        OnClientSendEventsStarted(EventArgs.Empty);
        //        firstEventReceveid = true;
        //    }

        //    buf_eventsList = new byte[dataSize];

        //    while (totRecvBytes < dataSize)
        //    {
        //        recvBytes = handledReceive(buf_eventsList, totRecvBytes, dataSize - totRecvBytes);
        //        totRecvBytes += recvBytes;
        //    }

        //    List<MacroEvent> list;
        //    using (MemoryStream ms = new MemoryStream(buf_eventsList))
        //        list = bf.Deserialize(ms) as List<MacroEvent>;

        //    return list;
        //}

        private void recEventsListLoop()
        {
            int recvBytes, totRecvBytes, dataSize;

            dataSize = recInt();
            //Console.WriteLine("First DataSize: {0}", dataSize);
            if (dataSize < 0)
                throw new ClientStoppedSendingEventsException();

            //do
            //{
            //Console.WriteLine("DataSize: {0}", dataSize);
            buf_eventsList = new byte[dataSize];

            recvBytes = 0;
            totRecvBytes = 0;
            while (totRecvBytes < dataSize)
            {
                recvBytes = handledReceive(buf_eventsList, totRecvBytes, dataSize - totRecvBytes);
                totRecvBytes += recvBytes;
            }

            using (MemoryStream ms = new MemoryStream(buf_eventsList))
                EventsBuffer.AddEvents((List<MacroEvent>)bf.Deserialize(ms));
            //} while ((dataSize = recInt()) >= 0);


        }

        private int recInt()
        {
            int recvBytes = 0;
            int totRecvBytes = 0;
            while (totRecvBytes < sizeof(int))
            {
                recvBytes = handledReceive(buf_int, sizeof(int) - totRecvBytes);
                totRecvBytes += recvBytes;
            }
            return BitConverter.ToInt32(buf_int, 0);
        }

        private void recLockedKeysStatus()
        {
            int recvBytes, totRecvBytes, dataSize;

            dataSize = recInt();
            if (dataSize < 0)
                throw new ClientStoppedSendingEventsException();

            buf_eventsList = new byte[dataSize];

            recvBytes = 0;
            totRecvBytes = 0;
            while (totRecvBytes < dataSize)
            {
                recvBytes = handledReceive(buf_eventsList, totRecvBytes, dataSize - totRecvBytes);
                totRecvBytes += recvBytes;
            }

            LockedKeysStatus lks;
            using (MemoryStream ms = new MemoryStream(buf_eventsList))
                lks = ((LockedKeysStatus)bf.Deserialize(ms));

            if (Control.IsKeyLocked(Keys.CapsLock) != lks.CapsLocked)
                KeyboardSimulator.KeyPress(Keys.CapsLock);
            if (Control.IsKeyLocked(Keys.NumLock) != lks.NumLocked)
                KeyboardSimulator.KeyPress(Keys.NumLock);
        }

        private void recClipBoard()
        {

            int recvBytes = 0;
            int totRecvBytes = 0;

            int dataSize = recInt();
            //Console.WriteLine("Received data size {0}", dataSize);

            byte[] buf_clipboard = new byte[dataSize];

            while (totRecvBytes < dataSize)
            {
                recvBytes = handledReceive(buf_clipboard, totRecvBytes, dataSize - totRecvBytes);
                totRecvBytes += recvBytes;
            }

            ClipboardManager.setClipboard(buf_clipboard);
            
        }

        public void SendClipBoard()
        {
            //aspetta che il nuovo thread generi il dictionary
            byte[] data = ClipboardManager.getClipboard();

            try
            {
                handler.Send(BitConverter.GetBytes(data.Length));
                handler.Send(data);
            }
            catch (Exception)
            {
                Console.WriteLine("Error during send serialized Dictionary dataObjects");
                Close();
            }

        }

        protected void OnClientConnected(EventArgs e)
        {
            if (ClientConnected != null)
                ClientConnected(this, e);
        }

        protected void OnClientDisconnected(EventArgs e)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, e);
        }

        protected void OnClientSendEventsStarted(EventArgs e)
        {
            if (ClientSendEventsStarted != null)
                ClientSendEventsStarted(this, e);
        }

        protected void OnClientSendEventsEnded(EventArgs e)
        {
            if (ClientSendEventsEnded != null)
                ClientSendEventsEnded(this, e);
        }

    }
}
