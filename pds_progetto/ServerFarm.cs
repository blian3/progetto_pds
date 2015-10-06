using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MouseKeyboardLibrary;

namespace pds_progetto
{
    [Serializable]
    public class Server_farm
    {

        public Dictionary<String, Server> server_list = new Dictionary<string, Server>();
        public HotKey HotKey_Exit;

        public List<HotKey> getHotKeys()
        {
            List<HotKey> list = new List<HotKey>();
            foreach (Server s in server_list.Values)
                list.Add(s.HotKey);

            return list;
        }

        public void updateServer(Server s)
        {
            server_list[s.Name].updateServer(s);
        }

        public void replaceServer(String old_name, Server s)
        {
            server_list.Add(s.Name, s);
            server_list.Remove(old_name);
        }

        public void saveNewServer(Server s)
        {
            server_list.Add(s.Name, s);
        }


    }

    [Serializable]
    public class Server
    {
        private String _name;
        private String _password;
        private HotKey _hotKey;

        private IPAddress _ip;
        private int _port;

        //boolean state, meglio fare l'enum Status
        private bool _authenticate;
        private bool _active;



        public Server(String name, String password, HotKey hk, IPAddress ip, int port)
        {
            _name = name;
            _password = password;
            _hotKey = hk;
            _ip = ip;
            _port = port;
        }

        public Server(String name, IPAddress ip, int port)
        {
            _name = name;
            _ip = ip;
            _port = port;
        }

        #region getters_setter

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public HotKey HotKey
        {
            get { return _hotKey; }
            set { _hotKey = value; }
        }

        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool Authenticate
        {
            get { return _authenticate; }
            set { _authenticate = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        #endregion


        public void updateServer(Server s)
        {
            _name = s.Name;
            _password = s.Password;
            _hotKey = s.HotKey;
            _ip = s.Ip;
            _port = s.Port;
        }
    }
}
