using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAsp.Redis
{
    public class Transaction : IDisposable
    {
        RedisClient client;
        string[] watch;
        /// <summary>
        /// 不适用WATCH
        /// </summary>
        /// <param name="client"></param>
        public Transaction(RedisClient client)
        {
            this.client = client;
            client.Multi();
            
        }

        /// <summary>
        /// 适用WATCH，必须设置WatchKey的值；必须key在同一台服务器上，所以分布式部署的这个方法不可用；
        /// </summary>
        /// <param name="client"></param>
        /// <param name="watchkey"></param>
        public Transaction(RedisClient client, params string[] watchkey)
        {
            this.client = client;
            this.watch = watchkey;
            client.Watch(watchkey);
            client.Multi();
        }
        /// <summary>
        /// 提交事务；
        /// </summary>
        public bool Commit()
        {
           return  client.Exec();
        }
        public void Discard()
        {
            client.Discard();
        }
        public void Dispose()
        {
            client.UnWatch();
            GC.SuppressFinalize(this);
        }
    }
}
