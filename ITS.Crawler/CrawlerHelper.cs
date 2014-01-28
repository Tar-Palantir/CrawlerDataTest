using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Globalization;

namespace ITS.Crawler
{
    /// <summary>
    /// 爬虫公共方法类
    /// </summary>
    public static class CrawlerHelper
    {
        /// <summary>
        /// 获取指定连接的域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDomainName(string url)
        {
            if (url == null)
            {
                throw new Exception("输入的url为空");
            }
            Regex reg = new Regex(@"(?<=://)([\w-]+\.)+[\w-]+(?<=/?)");
            return reg.Match(url, 0).Value.Replace("/", string.Empty);
        }

        /// <summary>
        /// 获取描述信息,去除Html标签
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        public static string StripHtml(string dataStr)
        {
            Regex objRegExp = new Regex("<(.|\n)+?>");
            string strOutput = objRegExp.Replace(dataStr, "");
            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");
            return strOutput;
        }

        /// <summary>
        /// 把所有空格变为一个空格
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        public static string GetTrim(string dataStr)
        {
            Regex r = new Regex(@"\s+");
            string wordsOnly = r.Replace(dataStr, " ");
            if (!string.IsNullOrEmpty(wordsOnly))
                return wordsOnly.Trim();
            return null;
        }

        /// <summary>
        /// 正则表达式去掉所有html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHTML(string Htmlstring)  //替换HTML标记
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*>;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }

        /// <summary>
        /// 根据正则表达式获取信息
        /// </summary>
        /// <param name="dataStr">源字符串</param>
        /// <param name="regular">正则表达式及选项</param>
        /// <param name="isAll">是否是所有信息，true：所有信息，false：要匹配的信息</param>
        /// <returns>根据正则表达式获取到的信息</returns>
        public static string GetDataByRegex(string dataStr, Regular regular, bool isAll)
        {
            Match titleMatch = Regex.Match(dataStr, regular.Expression, regular.Options);
            if (titleMatch != null && titleMatch.Groups != null && titleMatch.Groups.Count > 0)
            {
                if (isAll)
                {
                    return titleMatch.Groups[0].Value;
                }
                else
                {
                    return titleMatch.Groups[1].Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据URL获取html源文件
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>html源文件</returns>
        public static string GetHtmlData(string url)
        {
            string strWebData = string.Empty;
            string charSet = null;

            WebClient myWebClient = new WebClient();

            //创建WebClient实例myWebClient
            // 需要注意的：
            //有的网页可能下不下来，有种种原因比如需要cookie,编码问题等等
            //这是就要具体问题具体分析比如在头部加入cookie
            //webclient.Headers.Add("Cookie", cookie);
            //这样可能需要一些重载方法。根据需要写就可以了
            //获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。
            myWebClient.Credentials = CredentialCache.DefaultCredentials;
            //如果服务器要验证用户名,密码
            //NetworkCredential mycred = new NetworkCredential(struser, strpassword);
            //myWebClient.Credentials = mycred;
            //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号）
            byte[] myDataBuffer;
            try
            {
                myDataBuffer = myWebClient.DownloadData(url);
            }
            catch (Exception ex)
            {
                return "";
            }

            strWebData = Encoding.Default.GetString(myDataBuffer);
            //获取网页字符编码描述信息
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
            {
                //如果未获取到编码，则设置默认编码
                if (webCharSet == null || webCharSet == "")
                {
                    charSet = "UTF-8";
                }
                else
                {
                    charSet = webCharSet;
                }
            }
            if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
            {
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);
            }

            return strWebData;
        }

        /// <summary>
        /// 根据URL获取html源文件(Post)
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>html源文件</returns>
        public static string GetHtmlData(string url, string postParam)
        {
            string strWebData = string.Empty;
            string charSet = null;

            byte[] byteParam = Encoding.Default.GetBytes(postParam);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteParam.Length;

            Stream requestSream = request.GetRequestStream();
            requestSream.Write(byteParam, 0, byteParam.Length);
            requestSream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            charSet = response.CharacterSet;
            if (string.IsNullOrEmpty(charSet) || Encoding.GetEncoding(charSet) == null)
            {
                charSet = "gb2312";
            }

            StreamReader responseStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charSet));
            strWebData = responseStreamReader.ReadToEnd();

            return strWebData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniStr"></param>
        /// <returns></returns>
        public static string UnicodeToString(string uniStr)
        {
            string expression = @"\\u(?<Value>[a-zA-Z0-9]{4})";
            Regex regex = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return regex.Replace(uniStr, f => { return ((char)int.Parse(f.Groups["Value"].Value, NumberStyles.HexNumber)).ToString(); });
        }
    }
}
