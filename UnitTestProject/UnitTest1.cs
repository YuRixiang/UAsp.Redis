using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAsp.Redis;
using System.Text;
using System.Threading;
namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (Redis r = new Redis())
            {
                long t = DateTime.Now.Ticks / 10000;
                r.Cluster = false;
                //r.Auth = "password";
                r.configOption = "127.0.0.1:6379";
                r.Connect();
                //r.Set("mmm", "sdfsdfsd");
               Dictionary<string,string> xxx= r.HashGet("codeName");
                Dictionary<string, string> mmm = new Dictionary<string, string>() { { "k", "test" } , { "z", "test" }, { "x", "test" } };
                r.Set(mmm);
                r.HashSet("h", "yu", "kkk");
               string mx= r.HashGet("dmq:ba_area", "CN");
                //r.Del("k");
                for (int i = 0; i < 100; i++)
                {
                  //  r.Del("foo:test:MM" + i);
                   // r.Set("foo:test:MM" + i, "Ke" + i);
                    
                }

                // string str = r.SendCommand("Get", "foo:test");

                // Thread.Sleep(100);
                long e = DateTime.Now.Ticks / 10000;
                long m = e - t;
                //Console.Write(sb);
            }
        }
    }
}
