using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAsp.Redis;
using System.Text;
using System.Threading;
using System.Configuration;
namespace UnitTestProject
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            RedisMQClient mq = new RedisMQClient();

            mq.Producer("ceshi", "QQQ1");
            mq.Producer("ceshi", "QQQ2");
            mq.Producer("ceshi", "QQQ3");
            mq.Consumer("ceshi");
            mq.onListener += Mq_onListener;
            Thread.Sleep(1115000);
          // //  mq.Producer("ceshi", "QQQ2");
           // Thread.Sleep(10000);


        }

        private void Mq_onListener(object sender, string e)
        {
            string result = e;
        }
    }
}
