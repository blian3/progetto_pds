using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

using System.Collections.Generic;
using MouseKeyboardLibrary;


//TODO: throw eccezioni, devono essere gestite in Form1

public class ClientSocket
{
    private Socket socket = null;
    private BinaryFormatter bf = new BinaryFormatter();
    private bool connected = false;

    public event EventHandler Connected, Disconnected;

    public bool ConnectToServer(string ip, int port) {
        // TO-DO: void, con throw exception invece di return false
        try {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                socket.Connect(remoteEP);

                // manda password ed attende risposta dal server
                // se la risposta è OK -> connected
                // altrimenti -> eccezione che viene gestita dal form che mostra alert
                connected = true;

                OnConnect(EventArgs.Empty);

                Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());
                /* start-up protocol: password, risoluzione schermo, clipboard, capslock... */
                //Send_Resolution();  


            } catch (ArgumentNullException ane) {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                Close();
                return false;
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0}", se.ToString());
                Close();
                return false;
            } catch (Exception e) {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                Close();
                return false;
            }
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
            return false;
        }
        return true;
    }

    public void SendEventsList(List<MacroEvent> list)
    {
        if (!connected) return;

        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, list);
                byte[] data = ms.ToArray();
                socket.Send(BitConverter.GetBytes(data.Length));
                //Console.WriteLine("Sent data size {0}", data.Length);
                socket.Send(data);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine(e.StackTrace);
            OnDisconnect(EventArgs.Empty);
            Close();
        }
    }

    public void SendInt(int i)
    {
        if (!connected) return;
        try
        {
            socket.Send(BitConverter.GetBytes(i));
        }
        catch (Exception e)
        {
            OnDisconnect(EventArgs.Empty);
            Close();
        }
    }

    public void SendLockedKeysStatus()
    {
        if (!connected) return;
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, new LockedKeysStatus());
                byte[] data = ms.ToArray();
                socket.Send(BitConverter.GetBytes(data.Length));
                socket.Send(data);
            }
        }
        catch (Exception e)
        {
            OnDisconnect(EventArgs.Empty);
            Close();
        }
    }

    public void SendClipBoard()
    {
        //aspetta che il nuovo thread generi il dictionary
        byte[] data = ClipboardManager.getClipboard();
        
        try
        {
            socket.Send(BitConverter.GetBytes(data.Length));
            socket.Send(data);            
        }
        catch (Exception)
        {
            Console.WriteLine("Error during send serialized Dictionary dataObjects");
            Close();
        }

    }

    public void recClipBoard()
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
    
    private int recInt()
    {
        byte[] buf_int = new byte[sizeof(int)];
        int recvBytes = 0;
        int totRecvBytes = 0;
        while (totRecvBytes < sizeof(int))
        {
            recvBytes = handledReceive(buf_int, sizeof(int) - totRecvBytes);
            totRecvBytes += recvBytes;
        }
        return BitConverter.ToInt32(buf_int, 0);
    }

    private int handledReceive(byte[] buffer, int size)
    {
        return handledReceive(buffer, 0, size);
    }

    private int handledReceive(byte[] buffer, int offset, int size)
    {
        int ret = socket.Receive(buffer, offset, size, SocketFlags.None);
        // receive ritorna 0 se il socket viene chiuso mentre è in attesa
        // se il socket viene chiuso prima della receive => SocketException
        if (ret == 0)
        {
            OnDisconnect(EventArgs.Empty);
            Close();
            //throw new Exception("Receive returned 0 -> connection closed by client.");
        }
        return ret;
    }


    //public void Send_Resolution() {
    //    if (!connected) return;

    //    socket.Send(BitConverter.GetBytes(Screen.PrimaryScreen.Bounds.Height));
    //    socket.Send(BitConverter.GetBytes(Screen.PrimaryScreen.Bounds.Width));
    //}

    public void Close()
    {
        connected = false;

        if (socket == null)
            return;

        try
        {
            socket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            socket.Close();
            socket = null;
            OnDisconnect(EventArgs.Empty);
        }
    }

    protected void OnConnect(EventArgs e)
    {
        if (Connected != null)
            Connected(this, e);
    }

    protected void OnDisconnect(EventArgs e)
    {
        if (Disconnected != null)
            Disconnected(this, e);
    }


}