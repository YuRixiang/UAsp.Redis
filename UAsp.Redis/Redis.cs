using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Linq;
using log4net;
namespace UAsp.Redis
{
    public class Redis : IDisposable
    {
        private ILog log = LogManager.GetLogger(typeof(Redis));
        public SocketManager socketManger;
        public SocketManager.SocktInfo currentSocket = new SocketManager.SocktInfo();
        public bool Cluster;
        public string Auth;
        /// <summary>
        /// 连接配置""
        /// </summary>
        public string configOption;
        public void Connect()
        {
            socketManger = new SocketManager();
            string[] config = configOption.Split(',');

            for (int i = 0; i < config.Length; i++)
            {
                try
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
                    if (!string.IsNullOrEmpty(Auth))
                    {
                        string result = SendCommand("AUTH", Auth);
                        if (result != null & result.IndexOf("-", 0, 2) > -1)
                        {
                            throw new Exception("登录失败");
                        }
                    }
                }
                catch (SocketException o)
                {
                    log.Error(o);
                }

            }
        }
        #region 写数据
        private string SendDataCommand(byte[] data, string cmd, params object[] args)
        {
            StringBuilder resp = new StringBuilder();
            resp.AppendFormat("*{0}\r\n", 1 + args.Length + 1);
            resp.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);
            foreach (object arg in args)
            {
                string argStr = arg.ToString();
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp.AppendFormat("${0}\r\n{1}\r\n", argStrLength, argStr);
            }
            resp.AppendFormat("${0}\r\n", data.Length);

            byte[] r = Encoding.UTF8.GetBytes(resp.ToString());

            try
            {
                currentSocket.Socket.Send(r);
                if (data != null)
                {
                    currentSocket.Socket.Send(data);
                    currentSocket.Socket.Send(end_data);
                }
            }
            catch (SocketException ex)
            {
                socketManger.ClusterSocket.Remove(currentSocket);
                throw new Exception("连接以及断开");
                log.Error(0);
            }
            string sb = ReadLine();
            if (sb.Contains("MOVED") && Cluster)
            {
                string[] m = sb.ToString().Split(' ');
                string[] h = m[2].Split(':');
                string host = h[0];
                int port = int.Parse(h[1]);
                currentSocket = socketManger.ClusterSocket.FirstOrDefault(o => o.Port == port && o.Host == host);
                try
                {
                    currentSocket.Socket.Send(r);
                    if (data != null)
                    {
                        currentSocket.Socket.Send(data);
                        currentSocket.Socket.Send(end_data);
                    }
                }
                catch (SocketException ex)
                {
                    socketManger.ClusterSocket.Remove(currentSocket);
                    throw new Exception("连接以及断开");
                    log.Error(0);
                }
                sb = ReadLine();
                return sb;
            }
            else
            {
                return sb;
            }
        }

        private string SendMutilDataComand(Dictionary<string, string> data, string cmd)
        {
            if (Cluster)
            {
                throw new Exception("分布式集群不支持的方法");
            }
            else {
                StringBuilder resp = new StringBuilder();
                byte[] nl = Encoding.UTF8.GetBytes("\r\n");
                MemoryStream ms = new MemoryStream();

                foreach (KeyValuePair<string, string> arg in data)
                {
                    byte[] key = Encoding.UTF8.GetBytes(arg.Key);
                    byte[] val = Encoding.UTF8.GetBytes(arg.Value);
                    byte[] kLength = Encoding.UTF8.GetBytes("$" + key.Length + "\r\n");
                    byte[] k = Encoding.UTF8.GetBytes(arg.Key + "\r\n");
                    byte[] vLength = Encoding.UTF8.GetBytes("$" + val.Length + "\r\n");
                    ms.Write(kLength, 0, kLength.Length);
                    ms.Write(k, 0, k.Length);
                    ms.Write(vLength, 0, vLength.Length);
                    ms.Write(val, 0, val.Length);
                    ms.Write(nl, 0, nl.Length);
                }
                byte[] r = Encoding.UTF8.GetBytes("*" + (data.Count * 2 + 1) + "\r\n$4\r\nMSET\r\n");
                currentSocket.Socket.Send(r);
                currentSocket.Socket.Send(ms.ToArray());
                currentSocket.Socket.Send(end_data);


                return ReadLine();
            }

        }

        public bool Set(string key, string value)
        {
            string result = SendDataCommand(Encoding.UTF8.GetBytes(value), "SET", key);
            if (result == "+OK")
                return true;
            else
                return false;
        }
        public bool Set(Dictionary<string, string> dict)
        {
            string result = SendMutilDataComand(dict, "MSET");
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

        public bool HashSet(string key, string filed, string value)
        {
            string result = SendDataCommand(Encoding.UTF8.GetBytes(value), "HSET", key, filed);
            if (result == "+OK")
                return true;
            else
                return false;
        }
        #endregion
        public bool Del(string key)
        {
            string result = SendCommand("DEL", key);
            if (result == ":1")
                return true;
            else
                return false;
        }

        #region 读数据
        private string SendCommand(string cmd, params object[] args)
        {

            StringBuilder resp = new StringBuilder();
            resp.AppendFormat("*{0}\r\n", 1 + args.Length);
            resp.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);
            foreach (object arg in args)
            {
                string argStr = arg.ToString();
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp.AppendFormat("${0}\r\n{1}\r\n", argStrLength, argStr);
            }
            byte[] r = Encoding.UTF8.GetBytes(resp.ToString());
            try
            {
                try
                {
                    currentSocket.Socket.Send(r);
                }
                catch (SocketException o)
                {
                    log.Error(o);
                }
                string sb = ReadLine();

                if (sb.ToString().Contains("MOVED") && Cluster)
                {
                    string[] m = sb.ToString().Split(' ');
                    string[] h = m[2].Split(':');
                    string host = h[0];
                    int port = int.Parse(h[1]);
                    currentSocket = socketManger.ClusterSocket.FirstOrDefault(o => o.Port == port && o.Host == host);
                    try
                    {
                        currentSocket.Socket.Send(r);
                    }
                    catch (SocketException o)
                    {
                        log.Error(o);
                    }
                    sb = ReadLine();
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
            string result = SendCommand("GET", key);
            if (result.Contains("$-1"))
                return null;
            else
                result = ReadLine();
            return result;
        }
        public string HashGet(string key, string filed)
        {
            string result = SendCommand("HGET", key, filed);
            if (result == "$-1")
                return null;
            return ReadLine();
        }

        public Dictionary<string, string> HashGet(string key)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string result = SendCommand("HGETALL", key);
            if (result == "$-1")
                return null;
            int filedlen = int.Parse(result.Replace("*", ""));

            for (int i = 0; i < filedlen / 2; i++)
            {
                string s1 = ReadLine();
                string s2 = ReadLine();
                string s3 = ReadLine();
                string s4 = ReadLine();
                dic.Add(s2, s4);
            }
            return dic;
        }

        #endregion
        private string ReadLine()
        {
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
            return sb.ToString();
        }
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
