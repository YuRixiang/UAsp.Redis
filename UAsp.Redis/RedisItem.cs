using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using log4net;
namespace UAsp.Redis
{


    public class RedisItem
    {
        private readonly ILog log = LogManager.GetLogger(typeof(RedisItem));
        private System.Collections.Concurrent.ConcurrentStack<SocketManager> mClients = new System.Collections.Concurrent.ConcurrentStack<SocketManager>();
        public RedisItem(string host, int port, int count, int timeout = 2000, string password = null)
        {
            Host = host;
            Port = port;
            TimeOut = timeout;
            Password = password;
            for (int i = 0; i < count; i++)
            {
                Push(CreateClient());
            }

        }
        public void Push(SocketManager client)
        {
            if (client != null)
            {
                if (client.Client.Connected)
                {
                    mClients.Push(client);
                }
            }
        }
        public string Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }
        public int TimeOut { get; set; }
        public string Password { get; set; }
        public SocketManager CreateClient()
        {
            SocketManager manager = new SocketManager();
            Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sk.NoDelay = true;
            sk.ReceiveTimeout = TimeOut;
            sk.SendTimeout = TimeOut;
            try
            {
                if (ErrorClient.Client.ContainsKey(Host + ":" + Port))
                {
                    long time = DateTime.Now.Ticks / 10000;
                    if ((time - ErrorClient.Client[Host + ":" + Port]) < 3000)
                        return null;
                }
                sk.Connect(Host, Port);
                if (!sk.Connected)
                {
                    sk.Close();
                    sk = null;
                    log.Error(Host + ":" + Port + "服务器拒绝连接！");
                    return null;
                }
                manager.Client = sk;
                manager.Bstream = new BufferedStream(new NetworkStream(sk), 160 * 1024);
                ///登录
                if (!string.IsNullOrEmpty(Password))
                {
                    if (!Auth(Password, manager))
                        return null;
                }
                return manager;
            }
            catch
            {
                if (!ErrorClient.Client.ContainsKey(Host + ":" + Port))
                {
                    ErrorClient.Client.Add(Host + ":" + Port, DateTime.Now.Ticks / 10000);
                }                    
                else
                {
                    ErrorClient.Client[Host + ":" + Port] = DateTime.Now.Ticks / 10000;
                }
                log.Error(Host + ":" + Port + "服务器拒绝连接！");
                return null;
            }
        }
        public System.Collections.Concurrent.ConcurrentStack<SocketManager> Clients { get { return mClients; } }

        private bool Auth(string password, SocketManager socket)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*2\r\n$4\r\nAUTH\r\n${0}\r\n{1}\r\n", password.Length, password);
            byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
            socket.Client.Send(r);
            StringBuilder rd = new StringBuilder();
            int c;

            while ((c = socket.Bstream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                rd.Append((char)c);
            }
            return rd.ToString().Contains("OK");
        }
    }
    public class SocketManager
    {

        public BufferedStream Bstream { get; set; }
        public Socket Client { get; set; }

    }
}
