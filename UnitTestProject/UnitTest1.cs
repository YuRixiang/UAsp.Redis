using System;
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
                r.configOption = "192.168.6.134:6379,192.168.6.131:6380,192.168.6.131:6381,192.168.6.134:6380";
                r.Connect();
                
                r.SetEX("k", "v",30);
                r.Del("k");
                for (int i = 0; i < 100; i++)
                {
                    r.Set("foo:test:MM" + i, "Ke" + i);
                    
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
