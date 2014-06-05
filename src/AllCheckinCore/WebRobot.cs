using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public class WebRobot
    {
        private string cookie;

        public WebRobot()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        public string Get(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(url);
            }
        }

        public string PostAjax(string url, string data)
        {
            string result = "";
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.Accept, "*/*");
                client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
                client.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
                client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.154 Safari/537.36");
                client.Headers.Add(HttpRequestHeader.Referer, "http://zhaokaifang.com/");
                client.Headers.Add(HttpRequestHeader.Host, "zhaokaifang.com");
                client.Headers.Add("Origin", "http://zhaokaifang.com/");
                client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                client.Headers.Add(HttpRequestHeader.Cookie, "bdshare_firstime=1401624144679; CNZZDATA1000135535=306085563-1401624139-%7C1401624139");
                if (!string.IsNullOrEmpty(cookie))
                {
                    client.Headers.Add(HttpRequestHeader.Cookie, cookie);
                }

                result = client.UploadString(url, data);
                if (string.IsNullOrEmpty(cookie))
                {
                    var setCookie = client.ResponseHeaders["Set-Cookie"];
                    if (!string.IsNullOrEmpty(setCookie))
                    {
                        cookie = setCookie.ToString();
                        cookie = GetCookie(cookie);
                    }
                }

                return result;
            }
        }

        private string GetCookie(string CookieStr)
        {
            string result = "";
            string[] myArray = CookieStr.Split(',');

            if (myArray.Count() > 0)
            {
                result = "Cookie: ";
                foreach (var str in myArray)
                {
                    string[] CookieArray = str.Split(';');
                    result += CookieArray[0].Trim();
                    result += "; ";
                }

                result = result.Substring(0, result.Length - 2);
            }

            return result;
        } 
    }
}
