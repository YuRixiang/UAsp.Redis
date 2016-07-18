using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Text.RegularExpressions;
namespace UAsp.Redis
{
    public class RedisClient : RedisBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(RedisClient));

        public RedisClient()
        {
            base.Initializer();

        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            string m = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_GET, cmd.GetRead(), key);
            return m;
        }
        /// <summary>
        /// 查看集合
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] Smembers(string key)
        {
            string m = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SMEMBERS, null, key);
            string[] _tmp = Regex.Split(m, "\r\n");
            string[] result = new string[_tmp.Length / 2];
            int x = 0;
            for (int i = 0; i < _tmp.Length - 1; i += 2)
            {
                result[x] = _tmp[i + 1];
                x++;

            }
            return result;
        }
        /// <summary>
        /// 删除集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Srem(string key, string value)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SREM, null, key, value);
            return int.Parse(result) > 0;
        }
        /// <summary>
        /// 添加字符窜
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Set(string key, string value)
        {
            int result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SET, null, Encoding.UTF8.GetBytes(value), key);
            return result;

        }
        /// <summary>
        ///添加字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="express">有效时间秒</param>
        /// <returns></returns>
        public int Set(string key, string value, int express)
        {
            int result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SETEX, null, Encoding.UTF8.GetBytes(value), key, express.ToString());
            return result;

        }
        public int HashSet(string key, string filed, string value)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(filed, value);
            int result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_HSET, null, dic, key);
            return result;
        }
        public string HashGet(string key, string filed)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_HGET, null, key, filed);
            return result;
        }
        public Dictionary<string, string> HashGet(string key)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_HGETALL, null, key);
            string[] str = Regex.Split(result, "\r\n");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < str.Length - 1; i += 2)
            {
                dic.Add(str[i], str[i + 1]);
            }
            return dic;
        }
        public int HashDel(string key, string filed)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_HDEL, null, key, filed);
            return int.Parse(result);
        }
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int SAdd(string key, string value)
        {
            int result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SADD, null, Encoding.UTF8.GetBytes(value), key);
            return result;

        }
        /// <summary>
        /// 删除字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Del(string key)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_DEL, null, key);
            return int.Parse(result) > 0;
        }
        public void Save()
        {
            cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SAVE, null);

        }
        /// <summary>
        /// 通过给定可选的参数 section ，可以让命令只返回某一部分的信息:
        ///server: Redis服务器的一般信息
        ///clients: 客户端的连接部分
        /// memory: 内存消耗相关信息
        ///persistence: RDB和AOF相关信息
        ///stats: 一般统计
        ///replication: 主/从复制信息
        ///cpu: 统计CPU的消耗
        ///commandstats: Redis命令统计
        ///cluster: Redis集群信息
        ///keyspace: 数据库的相关统计
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public string Info(string section = "ALL")
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_INFO, null, section);

            return result;
        }

        public bool Expire(string key, int second)
        {
            cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_EXPIRE, null, key, second.ToString());
            return true;
        }
        public bool Exists(string key)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_EXISTS, null, key);
            return int.Parse(result) > 0;
        }

        /// <summary>
        /// 将 key 中储存的数字值增一。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Incr(string key)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_INCR, cmd.GetWrite(), key);
            return int.Parse(result);
        }
        #region 事务
        public bool Multi()
        {

            cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_MULTI, cmd.GetWrite());
            return true;
        }
        public bool Exec()
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_EXEC, cmd.GetWrite());
            //:8\r\n:10\r\n
            if (result.IndexOf(":") > -1)
                return true;
            else
                return false;
        }

        public bool Discard()
        {
            cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_DISCARD, cmd.GetWrite());
            return true;
        }
        public bool Watch(params string[] key)
        {
            string result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_WATCH, cmd.GetWrite(), key);
            if (result == "ok")
                return true;
            else
                return false;
        }
        public bool UnWatch()
        {
            cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_UNWATCH, cmd.GetWrite());
            return true;
        }
        #endregion


    }
}
