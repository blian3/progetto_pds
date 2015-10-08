using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    public enum ClipboardContentType
    {
        AUDIO,
        IMAGE,
        DATA,
        FILEDROPLIST,
        TEXT
    }

    [Serializable]
    public class ClipboardContent
    {
        public ClipboardContentType Type { get; set; }
        public byte[] Content { get; set; }
        public List<FileInfo> Files { get; set; }

        public ClipboardContent() { }
        public ClipboardContent(ClipboardContentType type, byte[] content)
        {
            Type = type;
            Content = content;
            Files = null;
        }
    }

    [Serializable]
    public class FileInfo
    {
        public string PathName { get; private set; }
        public byte[] Bytes { get; private set; }
        public bool isDir { get; private set; }

        public FileInfo(string pathname, string clipboardEntry, byte[] bytes)
        {
            this.Bytes = bytes;
            isDir = (bytes == null);
            this.PathName = getRelativePath(pathname, clipboardEntry);
        }

        public string getRelativePath(string pathname, string clipBoardEntry)
        {
            if (!pathname.Equals(clipBoardEntry))   // se sottocartella o file in sottocartella
                return (new DirectoryInfo(clipBoardEntry)).Name + pathname.Replace(clipBoardEntry, "");
            else if(isDir)      // entry nella clipboard è una cartella
                return (new DirectoryInfo(clipBoardEntry)).Name;
            else                // entry nella clipboard è un file
                return Path.GetFileName(pathname);

            //if (isDir)
            //{
            //    if (pathname.Equals(clipBoardEntry))
            //        return (new DirectoryInfo(clipBoardEntry)).Name; // + "\\"
            //    else // sottocartella
            //    {
            //        //string nomeCartellaClipboardEntry = (new DirectoryInfo(clipBoardEntry)).Name;
            //        //string pathRelativo = pathname.Replace(clipBoardEntry, "");
            //        //string pathRelativoConCartellaClipboardEntry = nomeCartellaClipboardEntry + pathRelativo;
            //        return (new DirectoryInfo(clipBoardEntry)).Name + pathname.Replace(clipBoardEntry, "");
            //    }
            //}
            //else
            //{
            //    if (pathname.Equals(clipBoardEntry))
            //        return Path.GetFileName(pathname);
            //    else // file nella sottocartella
            //    {
            //        //string nomeCartellaClipboardEntry = (new DirectoryInfo(clipBoardEntry)).Name;
            //        //string pathRelativo = pathname.Replace(clipBoardEntry, "");
            //        return (new DirectoryInfo(clipBoardEntry)).Name + pathname.Replace(clipBoardEntry, "");
            //    }
            //}
        }

    }

    public static class ClipboardManager
    {
        private static BinaryFormatter bf = new BinaryFormatter();

        public static byte[] getClipboard()
        {
            ClipboardContent cbContent = new ClipboardContent();
            Thread thread = new Thread(() =>
            {
                if (Clipboard.ContainsAudio())
                    cbContent = new ClipboardContent(ClipboardContentType.AUDIO, ObjectToByteArray(Clipboard.GetAudioStream()));
                else if (Clipboard.ContainsText())
                    cbContent = new ClipboardContent(ClipboardContentType.TEXT, ObjectToByteArray(Clipboard.GetText()));
                else if (Clipboard.ContainsImage())
                    cbContent = new ClipboardContent(ClipboardContentType.IMAGE, ObjectToByteArray(Clipboard.GetImage()));
                else if (Clipboard.ContainsFileDropList())
                {
                    StringCollection cbEntriesToSend = new StringCollection();
                    foreach (string cbEntry in Clipboard.GetFileDropList())
                    {
                        if (Directory.Exists(cbEntry))
                            cbEntriesToSend.Add((new DirectoryInfo(cbEntry)).Name);
                        else if (File.Exists(cbEntry))
                            cbEntriesToSend.Add(Path.GetFileName(cbEntry));
                    }
                    cbContent = new ClipboardContent(ClipboardContentType.FILEDROPLIST, ObjectToByteArray(cbEntriesToSend));
                    cbContent.Files = GetFiles(Clipboard.GetFileDropList());
                }
                else // caso generico
                    cbContent = new ClipboardContent(ClipboardContentType.DATA, ObjectToByteArray(Clipboard.GetDataObject()));
                
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return ObjectToByteArray(cbContent);
        }

        public static void setClipboard(byte[] bytes)
        {
            ClipboardContent cc = (ClipboardContent)ByteArrayToObject(bytes);
            Thread thread = new Thread(() =>
            {
                switch (cc.Type)
                {
                    case ClipboardContentType.AUDIO:
                        Clipboard.SetAudio(cc.Content);
                        return;
                    case ClipboardContentType.TEXT:
                        Clipboard.SetText((String)ByteArrayToObject(cc.Content));
                        return;
                    case ClipboardContentType.IMAGE:
                        Clipboard.SetImage((Image)ByteArrayToObject(cc.Content));
                        return;
                    case ClipboardContentType.FILEDROPLIST:
                        StringCollection paths = ConvertPaths((StringCollection)ByteArrayToObject(cc.Content));
                        ByteToFile(cc.Files);
                        Clipboard.SetFileDropList(paths);
                        return;
                    case ClipboardContentType.DATA:
                        IDataObject dataObject = (IDataObject)ByteArrayToObject(cc.Content);
                        DataObject cbData = new DataObject();
                        foreach (string format in dataObject.GetFormats())
                            cbData.SetData(format, false, dataObject.GetData(format, false));
                        Clipboard.SetDataObject(cbData, true);
                        return;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private static StringCollection ConvertPaths(StringCollection sc)
        {
            StringCollection ret = new StringCollection();
            foreach (string s in sc)
                ret.Add(Path.GetTempPath() + "\\" + s);

            return ret;
        }

        private static List<FileInfo> GetFiles(StringCollection pathnames)
        {
            List<FileInfo> list = new List<FileInfo>();

            foreach (string s in pathnames)
            {
                if (Directory.Exists(s))
                {
                    FileInfo fi_dir = new FileInfo(s, s, null);
                    list.Add(fi_dir);

                    foreach (string dir in Directory.GetDirectories(s, "*", SearchOption.AllDirectories))
                        list.Add(new FileInfo(dir, s, null));

                    foreach (string file in Directory.GetFiles(s, "*", SearchOption.AllDirectories))
                        list.Add(new FileInfo(file, s, File.ReadAllBytes(file)));
                }
                else if (File.Exists(s))
                {
                    FileInfo fi = new FileInfo(s, s, File.ReadAllBytes(s));
                    list.Add(fi);
                }
            }
            return list;
        }

        private static void ByteToFile(List<FileInfo> files)
        {
            // controllo eccezioni
            foreach (FileInfo fi in files)
            {
                if (fi.isDir)
                    Directory.CreateDirectory(Path.GetTempPath() + "\\" + fi.PathName);
                else
                    using (Stream file = File.OpenWrite(Path.GetTempPath() + "\\" + fi.PathName))
                        file.Write(fi.Bytes, 0, fi.Bytes.Length);
            }
        }


        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static object ByteArrayToObject(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
                return bf.Deserialize(ms);
        }
    }
}