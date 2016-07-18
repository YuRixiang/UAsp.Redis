using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAsp.Redis
{
    /// <summary>
    /// 命令 ，返回："$ ： + -"
    /// </summary>
    public class REDIS_COMMAND
    {
        /// <summary>
        /// 选择一个数据库，下标值从0开始，一个新连接默认连接的数据库是DB0。
        /// </summary>
        public const string REDIS_COMMAND_SELECT = "SELECT";
        /// <summary>
        /// 设置key对应字符串value，并且设置key在给定的seconds时间之后超时过期
        /// </summary>
        public const string REDIS_COMMAND_SETEX = "SETEX";
        /// <summary>
        /// 将键key设定为指定的“字符串”值。
        ///如果 key 已经保存了一个值，那么这个操作会直接覆盖原来的值，并且忽略原始类型。
        /// </summary>
        public const string REDIS_COMMAND_SET = "SET";
        /// <summary>
        /// 设置 key 指定的哈希集中指定字段的值。
        ///如果 key 指定的哈希集不存在，会创建一个新的哈希集并与 key 关联。
        ///如果字段在哈希集中存在，它将被重写。
        /// </summary>
        public const string REDIS_COMMAND_HSET = "HSET";
        /// <summary>
        /// 添加一个或多个指定的member元素到集合的 key中.指定的一个或者多个元素member 如果已经在集合key中存在则忽略.如果集合key 不存在，则新建集合key,并添加member元素到集合key中.
        /// 如果key 的类型不是集合则返回错误.
        /// </summary>

        public const string REDIS_COMMAND_SADD = "SADD";
        /// <summary>
        /// 对应给定的keys到他们相应的values上。MSET会用新的value替换已经存在的value，就像普通的SET命令一样。如果你不想覆盖已经存在的values，请参看命令MSETNX。
        /// MSET是原子的，所以所有给定的keys是一次性set的。客户端不可能看到这种一部分keys被更新而另外的没有改变的情况。
        /// 分布式集群不支持的方法
        /// </summary>
        public const string REDIS_COMMAND_MSET = "MSET";
        /// <summary>
        /// 如果删除的key不存在，则直接忽略。
        /// </summary>

        public const string REDIS_COMMAND_DEL = "DEL";

        /// <summary>
        /// 从 key 指定的哈希集中移除指定的域。在哈希集中不存在的域将被忽略。
        ///如果 key 指定的哈希集不存在，它将被认为是一个空的哈希集，该命令将返回0。
        /// </summary>
        public const string REDIS_COMMAND_HDEL = "HDEL";
        /// <summary>
        /// 返回 key 指定的哈希集中该字段所关联的值
        /// </summary>
        public const string REDIS_COMMAND_HGET = "HGET";
        /// <summary>
        /// 返回 key 指定的哈希集中所有的字段和值。返回值中，每个字段名的下一个是它的值，所以返回值的长度是哈希集大小的两倍
        /// </summary>
        public const string REDIS_COMMAND_HGETALL = "HGETALL";
        /// <summary>
        /// 返回key的value。如果key不存在，返回特殊值nil。如果key的value不是string，就返回错误，因为GET只处理string类型的values。
        /// </summary>
        public const string REDIS_COMMAND_GET = "GET";

        /// <summary>
        /// 获取集合
        /// </summary>
        public const string REDIS_COMMAND_SMEMBERS = "SMEMBERS";
        /// <summary>
        /// 在key集合中移除指定的元素. 如果指定的元素不是key集合中的元素则忽略 如果key集合不存在则被视为一个空的集合，该命令返回0.
        ///如果key的类型不是一个集合,则返回错误.
        /// </summary>
        public const string REDIS_COMMAND_SREM = "SREM";
        /// <summary>
        /// INFO命令以一种易于理解和阅读的格式，返回关于Redis服务器的各种信息和统计数值。
        ///通过给定可选的参数 section ，可以让命令只返回某一部分的信息:
        /// </summary>

        public const string REDIS_COMMAND_INFO = "INFO";

        /// <summary>
        /// 设置key的过期时间，超过时间后，将会自动删除该key。在Redis的术语中一个key的相关超时是不确定的。
        /// </summary>
        public const string REDIS_COMMAND_EXPIRE = "EXPIRE";
        /// <summary>
        /// 返回key是否存在。
        /// </summary>
        public const string REDIS_COMMAND_EXISTS = "EXISTS";
        /// <summary>
        /// 后台保存DB。会立即返回 OK 状态码。 Redis forks, 父进程继续提供服务以供客户端调用，子进程将DB数据保存到磁盘然后退出。如果操作成功，可以通过客户端命令LASTSAVE来检查操作结果。
        /// </summary>
        public const string REDIS_COMMAND_SAVE = "BGSAVE";
        /// <summary>
        /// 将 key 中储存的数字值增一。
        //如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
        //如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        //本操作的值限制在 64 位(bit)有符号数字表示之内。
        /// </summary>

        public const string REDIS_COMAND_INCR = "INCR";

        /// <summary>
        /// 标记一个事务块的开始。事务块内的多条命令会按照先后顺序被放进一个队列当中，最后由 EXEC 命令原子性(atomic)地执行。
        /// </summary>
        public const string REDIS_COMAND_MULTI = "MULTI";
        /// <summary>
        /// 执行所有事务块内的命令。
        /// </summary>
        public const string REDIS_COMAND_EXEC = "EXEC";
        /// <summary>
        /// 取消事务，放弃执行事务块内的所有命令。
        /// </summary>
        public const string REDIS_COMAND_DISCARD = "DISCARD";
        /// <summary>
        /// 监视一个(或多个) key ，如果在事务执行之前这个(或这些) key 被其他命令所改动，那么事务将被打断。
        /// </summary>
        public const string REDIS_COMAND_WATCH = "WATCH";
        /// <summary>
        /// 取消 WATCH 命令对所有 key 的监视。如果在执行 WATCH 命令之后， EXEC 命令或 DISCARD 命令先被执行了的话，那么就不需要再执行 UNWATCH 了。
        /// </summary>
        public const string REDIS_COMAND_UNWATCH = "UNWATCH";

        /// <summary>
        /// 命令将一个或多个值插入到列表头部。 如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。 当 key 存在但不是列表类型时，返回一个错误。
        /// </summary>

        public const string REDIS_COMAND_LPUSH = "LPUSH";

        /// <summary>
        /// Redis Lpop 命令用于移除并返回列表的第一个元素。
        /// </summary>
        public const string REDIS_COMAND_LPOP = "LPOP";


        /// <summary>
        /// Redis Blpop 命令移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        public const string REDIS_COMAND_BLPOP = "BLPOP";

        /// <summary>
        /// Redis Rpush 命令用于将一个或多个值插入到列表的尾部(最右边)。
        /// </summary>

        public const string REDIS_COMAND_RPUSH = "RPUSH";

        /// <summary>
        /// Redis Rpop 命令用于移除并返回列表的最后一个元素。
        /// </summary>
        public const string REDIS_COMAND_RPOP = "RPOP";

        /// <summary>
        /// Redis Brpop 命令移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        public const string REDIS_COMAND_BRPOP = "BRPOP";

    }
}
