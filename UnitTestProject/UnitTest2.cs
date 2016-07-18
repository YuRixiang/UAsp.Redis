using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAsp.Redis;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            RedisMQClient mq = new RedisMQClient();

            //mq.Producer("ceshi", "QQQ1");
            //mq.Producer("ceshi", "QQQ2");
            //mq.Producer("ceshi", "QQQ3");
            //mq.Consumer("ceshi");
            //mq.onListener += Mq_onListener;
            //Thread.Sleep(1115000);
            // //  mq.Producer("ceshi", "QQQ2");
            // Thread.Sleep(10000);


        }

        private void Mq_onListener(object sender, string e)
        {
            string result = e;
        }
        [TestMethod]
        public void Test()
        {
            List<ModelTest> list = new List<ModelTest>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new ModelTest { Address = "测试", ID = 1, Name = "余日祥" });
            }
            RedisExtend ex = new RedisExtend();
             ex.Set<List<ModelTest>>("My", list,1000);

            List<ModelTest> m = ex.Get<List<ModelTest>>("My");
        }
    }
    [Serializable]
    public class ModelTest
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
