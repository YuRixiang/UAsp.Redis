using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAsp.Redis;
using System.Text;
using System.Threading;
using System.Configuration;
namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (RedisClient r = new RedisClient())
            {
                r.Set("KK", "Value");
                
                //using (Transaction tr = new Transaction(r,"KK"))
                //{
                //    // r.Set("incr", "100");
                //    r.Incr("incr");
                //    int xc = r.Incr("incr");
                //   // r.UnWatch("KK");
                //   bool result=  tr.Commit();
                //}
                // r.Set("incr", "100");
                //string c = r.Get("incr");

                // r.SAdd("myset", "kkkss");
                 //r.SAdd("myset", "kkpss");
                 
                //string[] m = r.Smembers("myset");
                // r.HashSet("kk", "mu", "s");
                r.HashSet("singal", "1_12", "{\"Floor\":23,\"X\":1.0,\"Y\":2.0,\"Mac\":\"asdfasf\",\"Ticks\":9999,\"Rate\":-30.0}");
                // r.Exists("kk");
                // r.HashGet("kk");
                //// r.HashDel("kk", "mu");
                // r.Expire("kk", 100);
                // r.Save();
                //string m = r.Info("CPU");
            }

        }
    }
}

