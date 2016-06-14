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
               // r.Set("KK", "Value");
                
                using (Transaction tr = new Transaction(r,"KK"))
                {
                    // r.Set("incr", "100");
                    r.Incr("incr");
                    int xc = r.Incr("incr");
                   // r.UnWatch("KK");
                   bool result=  tr.Commit();
                }
                // r.Set("incr", "100");
                //string c = r.Get("incr");

                // r.SAdd("myset", "kkk");
                // r.SAdd("myset", "kkp");
                // r.HashSet("kk", "mu", "s");
                //r.HashSet("kk", "sss", "sss");
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

