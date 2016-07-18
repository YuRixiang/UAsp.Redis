/***************************************************
*创建人:TecD02
*创建时间:2016/7/18 10:28:35
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
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UAsp.Redis
{
    public class RedisExtend : RedisBase
    {
        public RedisExtend()
        {
            base.Initializer();
        }

        public T Get<T>(string key)
        {
            byte[] result;
            cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_GET, cmd.GetRead(), out result, key);
            BinaryFormatter bFormatter = new BinaryFormatter();
            return (T)bFormatter.Deserialize(new MemoryStream(result));

        }

        public bool Set<T>(string key, T value)
        {
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(mStream, value);
            byte[] data = mStream.GetBuffer();

            int m = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SET, cmd.GetWrite(), data, key);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="express">有效时间秒</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, int express)
        {
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(mStream, value);
            byte[] data = mStream.GetBuffer();

            int m = cmd.SendCommand(REDIS_COMMAND.REDIS_COMMAND_SET, cmd.GetWrite(), data, key, express.ToString());
            return true;
        }
    }
}
