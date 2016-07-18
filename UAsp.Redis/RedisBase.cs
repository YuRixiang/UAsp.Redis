/***************************************************
*创建人:TecD02
*创建时间:2016/7/16 18:57:28
*功能说明:<Function>
*版权所有:<Copyright>
*Frameworkversion:4.5.2
*CLR版本：4.0.30319.42000
***************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
namespace UAsp.Redis
{
    public class RedisBase : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(RedisBase));
        private bool cluster;
        private int db;
        private string password;
        public Command cmd;
        public void Initializer()
        {
            cmd = new Command();
            RedisSection config = (RedisSection)System.Configuration.ConfigurationManager.GetSection("redisSection");
            cluster = config.Cluster;
            db = config.Db;
            password = config.Password;
            ItemCollection read = config.Read;
            List<RedisItem> readlist = new List<RedisItem>();
            foreach (Host h in read)
            {
                RedisItem item = new RedisItem(h.Ip, h.Port, h.Pool, h.Timeout, password);
                if (item.Clients.Count > 0) { readlist.Add(item); }

            }
            cmd.Reads = readlist;
            ItemCollection write = config.Write;
            List<RedisItem> writelist = new List<RedisItem>();
            foreach (Host h in write)
            {
                RedisItem item = new RedisItem(h.Ip, h.Port, h.Pool, h.Timeout, password);
                if (item.Clients.Count > 0) { writelist.Add(item); }

            }
            cmd.Writes = writelist;
        }
        public RedisBase()
        {
            //Initializer();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RedisBase()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (RedisItem item in cmd.Writes)
                {
                    while (item.Clients.Count > 0)
                    {
                        SocketManager socket;
                        if (item.Clients.TryPop(out socket))
                        {
                            socket.Client.Disconnect(true);
                            socket.Client.Dispose();
                        }
                    }
                }
                foreach (RedisItem item in cmd.Reads)
                {
                    while (item.Clients.Count > 0)
                    {
                        SocketManager socket;
                        if (item.Clients.TryPop(out socket))
                        {
                            socket.Client.Disconnect(true);
                            socket.Client.Dispose();
                        }
                    }
                }
                //SendCommand("QUIT");
            }
        }
    }
}
