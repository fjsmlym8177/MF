using MF.Core.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Tests.Utilities
{
    [TestFixture]
    public class HttpHelperTests
    {
        private string BaseUrl = "http://localhost:53608/";
        private Dictionary<string, string> headers;


        [SetUp]
        public void SetUp()
        {
            headers = new Dictionary<string, string>();
            headers.Add("mike-api-version", "1");
        }

        [Test]
        public void GetCanUse()
        {
            //var v1Result = HttpHelper.Get<IList<string>>(BaseUrl + "api/HttpTest", null);
            //Assert.AreEqual(new string[] { "Get版本1" }, v1Result);

            var v1Result = HttpHelper.Get<IList<string>>(BaseUrl + "api/HttpTest", null);
            Assert.AreEqual(new string[] { "Get版本1" }, v1Result);

            //var result = HttpHelper.Get<IList<string>>(BaseUrl + "api/HttpTest", headers);
            //Assert.AreEqual(new string[] { "Get版本2" }, result);
        }

        [Test]
        public void DeleteCanUse()
        {
            var result1 = HttpHelper.Delete<object>(BaseUrl + "api/HttpTest/1", null);
            Assert.AreEqual("DELETE版本1", result1);


            //var result2 = HttpHelper.Delete<object>(BaseUrl + "api/HttpTest/1", headers);
            //Assert.AreEqual("DELETE版本2", result2);
        }

        [Test]
        public void PutCanUse()
        {
            var result1 = HttpHelper.Put<object>(BaseUrl + "api/HttpTest/1", new { ss = "xxxxx" }, null);
            Assert.AreEqual("PUT版本1", result1);


            //var result2 = HttpHelper.Put<object>(BaseUrl + "api/HttpTest/1", new { ss = "xxx" }, headers);
            //Assert.AreEqual("PUT版本2", result2);


        }

        [Test]
        public void PostCanUse()
        {
            //var result1 = HttpHelper.Post<object>(BaseUrl + "api/HttpTest", new { xxxx = "ccccccc", A = "Lym", B = "LRX" }, null);
            //Assert.AreEqual("POST版本1Lym LRX", result1);

            //var result2 = HttpHelper.Post<object>(BaseUrl + "api/HttpTest", new { }, headers);
            //Assert.AreEqual("POST版本2", result2);

            headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Basic OU9LV2JtVXJLcjoyZGRiZTc2NTkzOWZhZjI5ZDEwNTU3MDg3MzVjOGViNw==");
            HttpHelper.Post<object>("https://open-api-sandbox.shop.ele.me/token", new {
                grant_type = "authorization_code",
                code = "Lym",
                redirect_uri = "LRX",
                client_id= "client_id"
            }, headers, "application/x-www-form-urlencoded");
        }



    }
}
