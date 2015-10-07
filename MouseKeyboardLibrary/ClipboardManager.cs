using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.IO.Compression;

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
            if (isDir)
            {
                if (pathname.Equals(clipBoardEntry))
                    return (new DirectoryInfo(clipBoardEntry)).Name; // + "\\"
                else // sottocartella
                {
                    //string nomeCartellaClipboardEntry = (new DirectoryInfo(clipBoardEntry)).Name;
                    //string pathRelativo = pathname.Replace(clipBoardEntry, "");
                    //string pathRelativoConCartellaClipboardEntry = nomeCartellaClipboardEntry + pathRelativo;
                    return (new DirectoryInfo(clipBoardEntry)).Name + pathname.Replace(clipBoardEntry, "");
                }
            }
            else
            {
                if (pathname.Equals(clipBoardEntry))
                    return Path.GetFileName(pathname);
                else // file nella sottocartella
                {
                    //string nomeCartellaClipboardEntry = (new DirectoryInfo(clipBoardEntry)).Name;
                    //string pathRelativo = pathname.Replace(clipBoardEntry, "");
                    return (new DirectoryInfo(clipBoardEntry)).Name + pathname.Replace(clipBoardEntry, "");
                }
            }
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
                //else
                // gestire caso generico?
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
            {
                //ret.Add(Path.GetTempPath() + "\\" + Path.GetFileName(s));
                ret.Add(Path.GetTempPath() + "\\" + s);
            }
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






////private static Dictionary<string, Object> dataObjects = new Dictionary<string, Object>();
//private static byte[] data;



//public static byte[] getClipboardSerialized() {
//    Thread t = new Thread(() =>
//    {
//        //Serializable structure
//        Dictionary<string, Object> dataObjects = new Dictionary<string, Object>();

//        //Clipboard aux structure
//        IDataObject clipboardData = Clipboard.GetDataObject();

//    #region normalEntryClipboard

//    //Entries format of clipboard
//    string[] list_format = clipboardData.GetFormats();
//    Console.WriteLine("\n\n[CLIPBOARD] CountFormats: {0}", list_format.Length);
//    int index = 0;

//    //Each entry in Clipboard is inserted into dictionary dataObject
//    Console.WriteLine("GET CLIPBOARD ##################");
//    foreach (string format in list_format)
//    {
//        object clipboardItem = clipboardData.GetData(format);
//        if (clipboardItem != null && clipboardItem.GetType().IsSerializable)
//        {
//            Console.WriteLine("#{0}\tFormat: {1}\t\t\t\tContent:{2}", index, format, clipboardItem.ToString());
//            index++;
//            dataObjects.Add(format, clipboardItem);
//        }
//        if (clipboardItem == null)
//            Console.WriteLine("null {0}", format);
//        else if (!clipboardItem.GetType().IsSerializable)
//            Console.WriteLine("not serializable {0}\t\t{1}", format, clipboardItem.ToString());
//    }

//    #endregion

//    #region fileEntryClipboard

//    //Veriry presence of file
//    System.Collections.Specialized.StringCollection returnList = null;
//    if (Clipboard.ContainsFileDropList())
//    {
//        returnList = Clipboard.GetFileDropList();
//    }


//    try
//    {

//        if (returnList != null)
//        {
//            //structure to send in FILETRANSFER enrty
//            List<byte[]> list_file = new List<byte[]>();

//            //Ci sono uno o più file da trasferire
//            foreach (string filePath in returnList)
//            {
//                //allocate byte array
//                byte[] bytes_file;

//                ////get filename
//                //string fileName = Path.GetFileName(filePath);
//                //Console.WriteLine("Filename sended: "+fileName);

//                using (FileStream fs_file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
//                {

//                    //TODO : grandezza buffer. 
//                    byte[] bytes = new byte[32768];

//                    //readSafe 
//                    using (MemoryStream ms = new MemoryStream())
//                    {
//                        while (true)
//                        {
//                            int read = fs_file.Read(bytes, 0, bytes.Length);
//                            if (read <= 0)
//                                break;
//                            ms.Write(bytes, 0, read);
//                        }
//                        bytes_file = ms.ToArray();
//                    }
//                }
//                //add to list file
//                list_file.Add(bytes_file);
//                Console.WriteLine("ADDED NEW FILE TO FILETRANSFER: size: " + bytes_file.Length);
//            }


//            //Byte array into dictionary dataObject
//            dataObjects.Add("FILETRANSFER", list_file);

//        }
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine("EXCPETION WHILE READING FILESTREAM");
//    }
//        #endregion

//    #region imageClipboard
//    Image img = null;
//    if (Clipboard.ContainsImage())
//    {
//        img = Clipboard.GetImage();
//    }

//    if (img != null)
//    {
//        dataObjects.Add("IMAGETRANSFER", img);
//        Console.WriteLine("img added to dataobjectes");
//    }
//    #endregion


//    #region serializeDataObjects
//    try
//    {
//        using (MemoryStream ms = new MemoryStream())
//        {
//            if (dataObjects.ContainsKey("FILETRANSFER"))
//                Console.WriteLine("#[CLIPBOOOOOOOOOARD] mbare ");

//            //serialize dictionary in byte[]
//            bf.Serialize(ms, dataObjects);
//            data = ms.ToArray();
//        }
//    }
//    catch (SerializationException)
//    {
//        Console.WriteLine("Error during serialization Dictionary dataObjects");
//    }
//    #endregion







//    });
//    t.SetApartmentState(ApartmentState.STA);
//    t.Start();
//    t.Join();

//    return data;
//}



//public static void setClipboardSerialized(byte[] data){
//    Thread t = new Thread(() => {

//        #region DeserializeClipboard
//        Dictionary<string, Object> dataObjects = new Dictionary<string, Object>();
//        using (MemoryStream ms = new MemoryStream(data))
//            dataObjects = bf.Deserialize(ms) as Dictionary<string, Object>;

//        #endregion

//        #region normalEntryClipboard
//        DataObject clipboardData = new DataObject();
//        int index = 0;

//        Console.WriteLine("SET CLIPBOARD ##################");
//        //Retrieve all format and all value in dictionary
//        foreach (string key in dataObjects.Keys)
//        {
//            if (!key.Equals("FILETRANSFER"))
//            {
//                clipboardData.SetData(key, dataObjects[key]);
//                Console.WriteLine("#{0}\tFormat: {1}\t\t\t\tContent:{2}", index, key, dataObjects[key].ToString());
//                index++;
//            }
//        }

//        #endregion

//        //Clipboard Restored
//        //Clipboard.SetDataObject(clipboardData);

//        #region fileEntryClipboard
//        //Wait a moment. Is there file in there?
//        System.Collections.Specialized.StringCollection returnList = null;

//        if (Clipboard.ContainsFileDropList())
//        {
//            Console.WriteLine("Clipboard contais file drop list");
//            List<byte[]> list_file = new List<byte[]>();
//            string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//            returnList = Clipboard.GetFileDropList();

//            if (dataObjects.ContainsKey("FILETRANSFER"))
//            {
//                list_file = (List<byte[]>)dataObjects["FILETRANSFER"];
//            }

//            StringCollection sc = new StringCollection();
//            if (list_file.Count == returnList.Count)
//            {
//                foreach (byte[] file in list_file)
//                {
//                    string s = desktop_path + "\\" + Path.GetFileName(returnList[list_file.IndexOf(file)]);
//                    sc.Add(s);
//                    using (FileStream fs = new FileStream(s, FileMode.Create, FileAccess.ReadWrite))
//                    {
//                        fs.Write(file, 0, file.Length);
//                        Console.WriteLine("Received size file #{0} in clipboard: {1} \t{2}", list_file.IndexOf(file), file.Length, s);
//                    }

//                }

//            }
//            else
//            {
//                Console.WriteLine("Problemiiiiiii: list_file.Count != returnList.Count {0}!={1}", list_file.Count, returnList.Count);
//            }

//            Clipboard.SetFileDropList(sc);

//        }
//        #endregion

//        if (dataObjects.ContainsKey("IMAGETRANSFER"))
//        {
//            Image img1 = dataObjects["IMAGETRANSFER"] as Image;

//            Clipboard.SetImage(img1);
//        }



//        //Serializable structure
//        Dictionary<string, Object> dataObjects2 = new Dictionary<string, Object>();

//        //Clipboard aux structure
//        IDataObject clipboardData2 = Clipboard.GetDataObject();


//        //Entries format of clipboard
//        string[] list_format2 = clipboardData2.GetFormats();
//        Console.WriteLine("\n\n[CLIPBOARD] CountFormats: {0}", list_format2.Length);
//        int index2 = 0;

//        //Each entry in Clipboard is inserted into dictionary dataObject
//        Console.WriteLine("GET CLIPBOARD DOPO SET  ##################");
//        foreach (string format in list_format2)
//        {
//            object clipboardItem = clipboardData2.GetData(format);
//            if (clipboardItem != null && clipboardItem.GetType().IsSerializable)
//            {
//                Console.WriteLine("#{0}\tFormat: {1}\t\t\t\tContent:{2}", index2, format, clipboardItem.ToString());
//                index2++;
//                dataObjects2.Add(format, clipboardItem);
//            }
//            if (clipboardItem == null)
//                Console.WriteLine("null {0}", format);
//            else if (!clipboardItem.GetType().IsSerializable)
//                Console.WriteLine("not serializable {0}\t\t{1}", format, clipboardItem.ToString());
//        }


//    });

//    t.SetApartmentState(ApartmentState.STA);
//    t.Start();
//    t.Join();
//}
//    }
//}
