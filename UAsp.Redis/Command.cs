using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using log4net;
namespace UAsp.Redis
{
    public class Command
    {
        private readonly ILog log = LogManager.GetLogger(typeof(Command));
        private readonly byte[] _end_data = new byte[] { (byte)'\r', (byte)'\n' };
        public IList<RedisItem> Reads
        {
            get;
            set;
        }

        public IList<RedisItem> Writes
        {
            get; set;
        }

        private int readIndex = 0;

        private int writeIndex = 0;

        public SocketManager GetWrite()
        {
            RedisItem item;
            SocketManager socket;
            for (int i = 0; i < Writes.Count; i++)
            {
                item = Writes[writeIndex % Writes.Count];
                if (item.Clients.Count < 1)
                    continue;
                if (item.Clients.TryPop(out socket))
                {
                    if (socket.Client.Connected)
                    {
                        item.Clients.Push(socket);
                        return socket;
                    }
                }
                writeIndex++;
            }
            log.Error("没有可写入的的服务器");
            throw new Exception("没有可用的服务器");
        }
        public SocketManager GetRead()
        {
            RedisItem item;
            SocketManager socket;
            for (int i = 0; i < Reads.Count; i++)
            {
                item = Reads[readIndex % Reads.Count];
                if (item.Clients.Count < 1)
                    continue;
                if (item.Clients.TryPop(out socket))
                {
                    if (socket.Client.Connected)
                    {
                        item.Clients.Push(socket);
                        return socket;
                    }
                }
                readIndex++;
            }
            log.Error("没有可读取的的服务器");
            throw new Exception("没有可用的服务器");
        }
        public string SendCommand(string command, SocketManager read, params string[] args)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n", args != null ? args.Length + 1 : 1);

            var cmd = command.ToString();
            sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

            if (args != null)
                foreach (var arg in args)
                {
                    sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
                }

            byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
            if (read == null)
            {
                read = GetRead();
            }
            var socket = read.Client;
            try
            {

                socket.Send(r);
                string msg = ReadLine(read);
                log.Info(msg);
                if (msg.Contains("-MOVED"))
                {
                    string[] m = msg.ToString().Split(' ');
                    string[] h = m[2].Split(':');
                    string host = h[0];
                    int port = int.Parse(h[1]);
                    RedisItem items = GetItem(Reads, host, port);
                    items.Clients.TryPop(out read);
                    items.Clients.Push(read);
                    socket = read.Client;
                    socket.Send(r);
                    msg = ReadLine(read);
                    log.Info(msg);
                }
                if (!msg.Contains("$-1") && !msg.Contains("-MOVED"))
                {
                    if (msg.IndexOf("$") > -1)
                    {
                        int l = int.Parse(msg.Replace("$", ""));
                        msg = ReadLine(read) + "\r\n";
                        if (msg.Length > l)
                            return msg;
                        while (true)
                        {
                            msg = msg + ReadLine(read) + "\r\n";
                            if (msg.Length > l)
                                break;
                        }
                        log.Info(msg);
                        return msg;
                    }
                    ///读取对象长度
                    if (msg.IndexOf("*") > -1)
                    {
                        string result = string.Empty;
                        int l = int.Parse(msg.Replace("*", ""));
                        if (msg.IndexOf("-") == 1)
                        {
                            return "0";
                        }
                        for (int i = 0; i < l; i++)
                        {
                            string val = ReadLine(read);
                            result = result + val + "\r\n";
                        }

                        return result;
                    }
                    //操作影响行数 删除
                    if (msg.IndexOf(":") > -1)
                    {
                        int result = int.Parse(msg.Replace(":", ""));

                        return result.ToString();
                    }
                    if (msg.IndexOf("+") > -1)
                    {
                        return "1";
                    }
                    if (msg.IndexOf("-") == 0)
                    { return "0"; };
                }
            }
            catch (SocketException e)
            {

                return "0";
            }

            return "0";
        }

        public int SendCommand(string command, SocketManager write, byte[] datas, params string[] args)
        {

            StringBuilder sb = new StringBuilder();

            int acount = args != null ? args.Length + 1 : 1;
            acount += datas != null && datas.Length > 0 ? 1 : 0;

            sb.AppendFormat("*{0}\r\n", acount);

            var cmd = command.ToString();
            sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

            if (args != null)
                foreach (var arg in args)
                {
                    sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
                }
            sb.AppendFormat("${0}\r\n", datas.Length);
            log.Info(sb);
            byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
            if (write == null)
            {
                write = GetWrite();

            }
            var socket = write.Client;

            try
            {

                socket.Send(r);


                if (datas != null && datas.Length > 0)
                {
                    socket.Send(datas);
                    socket.Send(_end_data);
                }

                string msg = ReadLine(write);
                log.Info(msg);
                if (msg.Contains("-MOVED"))
                {
                    string[] m = msg.ToString().Split(' ');
                    string[] h = m[2].Split(':');
                    string host = h[0];
                    int port = int.Parse(h[1]);
                    RedisItem items = GetItem(Writes, host, port);
                    items.Clients.TryPop(out write);
                    items.Clients.Push(write);
                    socket = write.Client;
                    socket.Send(r);
                    if (datas != null && datas.Length > 0)
                    {
                        socket.Send(datas);
                        socket.Send(_end_data);
                    }
                    msg = ReadLine(write);
                    log.Info(msg);
                }
                if (msg.IndexOf("+") > -1)
                { return 1; }
                if (msg.IndexOf(":") > -1)
                {
                    return int.Parse(msg.Replace(":", ""));
                }

            }
            catch (SocketException e)
            {

                return 0;
            }
            return 0;
        }

        public int SendCommand(string command, SocketManager write, IDictionary<string, byte[]> datas, params string[] args)
        {

            // http://redis.io/topics/protocol

            StringBuilder sb = new StringBuilder();

            int acount = args != null ? args.Length + 1 : 1;
            acount += datas != null && datas.Count > 0 ? datas.Count * 2 : 0;

            sb.AppendFormat("*{0}\r\n", acount);

            var cmd = command.ToString();
            sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

            if (args != null)
                foreach (var arg in args)
                {
                    sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
                }
            log.Info(sb);
            MemoryStream ms = new MemoryStream();

            byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
            ms.Write(r, 0, r.Length);

            if (datas != null && datas.Count > 0)
            {
                foreach (var data in datas)
                {
                    r = Encoding.UTF8.GetBytes(string.Format("${0}\r\n{1}\r\n", data.Key.Length, data.Key));
                    ms.Write(r, 0, r.Length);
                    r = Encoding.UTF8.GetBytes(string.Format("${0}\r\n", data.Value.Length));
                    ms.Write(r, 0, r.Length);
                    ms.Write(data.Value, 0, data.Value.Length);
                    ms.Write(_end_data, 0, 2);
                }
            }

            if (write == null)
            {
                write = GetRead();
            }
            var socket = write.Client;
            try
            {

                socket.Send(ms.ToArray());

                string msg = ReadLine(write);

                if (msg.Contains("-MOVED"))
                {
                    string[] m = msg.ToString().Split(' ');
                    string[] h = m[2].Split(':');
                    string host = h[0];
                    int port = int.Parse(h[1]);
                    RedisItem items = GetItem(Writes, host, port);
                    items.Clients.TryPop(out write);
                    items.Clients.Push(write);
                    socket = write.Client;
                    socket.Send(ms.ToArray());
                    msg = ReadLine(write);
                    log.Info(msg);
                }
                if (msg.IndexOf("+") > -1)
                { return 1; }
                if (msg.IndexOf(":") > -1)
                {
                    return int.Parse(msg.Replace(":", ""));
                }
            }
            catch (SocketException e)
            {

                throw e;
            }
            return 0;
        }

        public int SendCommand(string command, SocketManager read, IDictionary<string, string> datas, params string[] args)
        {
            var result = new Dictionary<string, byte[]>();

            foreach (var kv in datas)
                result.Add(kv.Key, Encoding.UTF8.GetBytes(kv.Value));

            return SendCommand(command, read, result, args);
        }
        private string ReadLine(SocketManager manager)
        {

            StringBuilder sb = new StringBuilder();
            int c;
            try
            {
                while ((c = manager.Bstream.ReadByte()) != -1)
                {
                    if (c == '\r')
                        continue;
                    if (c == '\n')
                        break;
                    sb.Append((char)c);
                }
            }
            catch (Exception ex) { log.Error(ex); }
            return sb.ToString();
        }

        private RedisItem GetItem(IList<RedisItem> client, string host, int port)
        {
            if (client.Count() == 0)
                throw new Exception("没有可用的Redis");
            RedisItem item = client.FirstOrDefault(o => o.Host == host & o.Port == port);
            if (item == null)
            {
                item = new RedisItem(host, port, client[0].Clients.Count, client[0].TimeOut, client[0].Password);
                client.Add(item);
            }
            return item;
        }

    }
}
