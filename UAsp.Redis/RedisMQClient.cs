/***************************************************
*创建人:TecD02
*创建时间:2016/7/16 19:19:34
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
namespace UAsp.Redis
{
    public class RedisMQClient : RedisBase
    {

        public event EventHandler<string> onListener;
        public RedisMQClient()
        {
            base.Initializer();
        }

        public int Producer(string key, string value)
        {
            int result = cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_RPUSH, cmd.GetWrite(), Encoding.UTF8.GetBytes(value), key);
            return result;
        }

        public void Consumer(string key)
        {

            Task task = new Task(CreateConsumer, key);
            task.Start();


        }
        private void CreateConsumer(object key)
        {
            while (true)
            {
                string result = base.cmd.SendCommand(REDIS_COMMAND.REDIS_COMAND_BLPOP, cmd.GetRead(), key.ToString(), 5000.ToString());

                string[] split = Regex.Split(result, "\r\n");
                if (split.Length >= 4)
                {
                    onListener?.Invoke(this, split[3]);
                }

            }

        }
    }
}
