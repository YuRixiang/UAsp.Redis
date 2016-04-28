using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Linq;
namespace UAsp.Redis
{
    public class Redis : IDisposable
    {

        public SocketManager socketManger;
        public SocketManager.SocktInfo currentSocket = new SocketManager.SocktInfo();
        public bool Cluster;
        public string configOption;
        public void Connect()
        {
            socketManger = new SocketManager();
            string[] config = configOption.Split(',');

            for (int i = 0; i < config.Length; i++)
            {
                SocketManager.SocktInfo info = new SocketManager.SocktInfo();
                string[] _temp = config[i].Split(':');
                info.Host = _temp[0];
                info.Port = int.Parse(_temp[1]);
                Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sk.NoDelay = true;
                sk.Connect(info.Host, info.Port);
                if (!sk.Connected)
                {
                    sk.Close();
                    sk = null;
                    break;
                }
                info.Socket = sk;
                info.Enable = true;
                info.Bstream = new BufferedStream(new NetworkStream(sk), 16 * 1024);
                socketManger.ClusterSocket.Add(info);
                if (socketManger.ClusterSocket.Count == 1)
                {
                    currentSocket = info;
                }

            }
        }
        #region 写数据
        private string SendDataCommand(byte[] data, string cmd, params object[] args)
        {
            string resp = "*" + (1 + args.Length + 1).ToString() + "\r\n";
            resp += "$" + cmd.Length + "\r\n" + cmd + "\r\n";
            foreach (object arg in args)
            {
                string argStr = arg.ToString();
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp += "$" + argStrLength + "\r\n" + argStr + "\r\n";
            }
            resp += "$" + data.Length + "\r\n";

            byte[] r = Encoding.UTF8.GetBytes(resp);


            currentSocket.Socket.Send(r);
            if (data != null)
            {
                currentSocket.Socket.Send(data);
                currentSocket.Socket.Send(end_data);
            }
            StringBuilder sb = new StringBuilder();
            int c;

            while ((c = currentSocket.Bstream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                sb.Append((char)c);
            }
            if (sb.ToString().Contains("MOVED") && Cluster)
            {
                string[] m = sb.ToString().Split(' ');
                string[] h = m[2].Split(':');
                string host = h[0];
                int port = int.Parse(h[1]);
                currentSocket = socketManger.ClusterSocket.FirstOrDefault(o => o.Port == port && o.Host == host);
                currentSocket.Socket.Send(r);
                if (data != null)
                {
                    currentSocket.Socket.Send(data);
                    currentSocket.Socket.Send(end_data);
                }
                sb = new StringBuilder();
                c = 0;

                while ((c = currentSocket.Bstream.ReadByte()) != -1)
                {
                    if (c == '\r')
                        continue;
                    if (c == '\n')
                        break;
                    sb.Append((char)c);
                }
                return sb.ToString();
            }
            else
            {
                return sb.ToString();
            }
        }
        public bool Set(string key, string value)
        {
            SendDataCommand(Encoding.UTF8.GetBytes(value), "SET", key);
            return true;
        }
        public bool SetEX(string key, string value, int second)
        {
            string result = SendDataCommand(Encoding.UTF8.GetBytes(value), "SETEX", key, second);
            if (result == "+OK")
                return true;
            else
                return false;
        }
        public bool Del(string key)
        {
            string result = SendCommand("DEL", key);
            if (result == ":1")
                return true;
            else
                return false;
        }
        #endregion
        #region 读数据
        private string SendCommand(string cmd, params object[] args)
        {

            string resp = "*" + (1 + args.Length).ToString() + "\r\n";
            resp += "$" + cmd.Length + "\r\n" + cmd + "\r\n";
            foreach (object arg in args)
            {
                string argStr = arg.ToString();
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp += "$" + argStrLength + "\r\n" + argStr + "\r\n";
            }

            byte[] r = Encoding.UTF8.GetBytes(resp);
            try
            {

                currentSocket.Socket.Send(r);
                StringBuilder sb = new StringBuilder();
                int c;

                while ((c = currentSocket.Bstream.ReadByte()) != -1)
                {
                    if (c == '\r')
                        continue;
                    if (c == '\n')
                        break;
                    sb.Append((char)c);
                }
                if (sb.ToString().Contains("MOVED")&& Cluster)
                {
                    string[] m = sb.ToString().Split(' ');
                    string[] h = m[2].Split(':');
                    string host = h[0];
                    int port = int.Parse(h[1]);
                    currentSocket = socketManger.ClusterSocket.FirstOrDefault(o => o.Port == port && o.Host == host);
                    currentSocket.Socket.Send(r);
                    sb = new StringBuilder();
                    c = 0;

                    while ((c = currentSocket.Bstream.ReadByte()) != -1)
                    {
                        if (c == '\r')
                            continue;
                        if (c == '\n')
                            break;
                        sb.Append((char)c);
                    }
                }
                return sb.ToString();
            }
            catch (SocketException)
            {
                currentSocket.Socket.Close();
                currentSocket.Socket = null;
                return null;

            }
        }
        public string Get(string key)
        {
            return SendCommand("GET", key);
        }
        #endregion
        byte[] end_data = new byte[] { (byte)'\r', (byte)'\n' };
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Redis()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //SendCommand("QUIT");
            }
        }
        public class SocketManager
        {
            private List<SocktInfo> clustersocket = new List<SocktInfo>();
            public class SocktInfo
            {
                public Socket Socket { get; set; }
                public int Solt { get; set; }
                public string Host { get; set; }

                public int Port { get; set; }
                public bool Enable { get; set; }
                public BufferedStream Bstream { get; set; }
            }

            public List<SocktInfo> ClusterSocket { get { return this.clustersocket; } set { clustersocket = value; } }

        }

    }
}
