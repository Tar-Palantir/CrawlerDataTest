using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web.Configuration;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    /// <summary>
    /// 中国开发区信息网抓取类
    /// 地址：http://www.cdz.cn
    /// </summary>
    internal class CdzNew : TestBaseCrawler
    {
        #region 变量定义

        /// <summary>
        /// 信息采集于中国开发区信息网
        /// </summary>
        public override string InfoSource { get { return "中国开发区信息网"; } }

        /// <summary>
        /// 链接的域名
        /// </summary>
        public override string DomainName { get { return "www.cdz.cn"; } }
        #endregion

        #region 启动爬虫程序（初始化各种参数）

        protected override void ListPageRegAndParamInitialize()
        {
            PageGetRegex = new Regular("&ScrollAction=(?<PageNum>\\d*)", DefaultRegexOptions);

            HrefRegex = new Regular("<a href=\"(?<Url>.+?)\" target=\"_blank\" class=\"text1\" title=\"(?<Title>.+?)\">[\\s\\S]*?<span id=\"doctime_dr\">(?<Date>[\\s\\S]*?)</span>", DefaultRegexOptions);
        }

        protected override void ContentPageRegAndParamInitialize()
        {
            TotalContentCountRegex = new Regular("[\\s\\S]*?页/共(?<PageCount>\\d{1,9}?)页[\\s\\S]*?", DefaultRegexOptions);

            TitleRegex = new Regular("(<td[\\s\\S]*?class=\"wzym_title_new\"[\\s\\S]*?>([\\s\\S]*?)</td>)|(<title>([\\s\\S]*?)</title>)", DefaultRegexOptions);

            PublishDateRegex = new Regular("<td[\\s\\S]*?class=\"wzym_title2_new\"[\\s\\S]*?>(?<Date>[\\s\\S]*?)</td>", DefaultRegexOptions);

            PublishSourceRegex = new Regular("<td[\\s\\S]*?class=\"rj_text4\"[\\s\\S]*?>(?<Source>[\\s\\S]*?)</td>", DefaultRegexOptions);
        }

        /// <summary>
        /// 启动爬虫程序
        /// </summary>
        /// <param name="url"></param>
        public override string Start<TOption>(Uri url, TOption option)
        {
            #region 配置爬虫选项
            option.IsCrawlerByNum = true;
            option.CrawlerNum = 5;
            option.MaxCrawlerNum = 20;
            option.ListParamMethod = ParamTransMethod.Get;
            #endregion

            return base.Start(url, option);
        }
        #endregion

        #region 获取所有链接

        protected override bool IsPageHasList(string strWebData)
        {
            return !string.IsNullOrEmpty(strWebData) && strWebData.Contains("<input type=\"button\" name=\"Submit\" class=\"an_button1\" value=\"首 页\" disabled>");
        }

        protected override string UrlToAbsUrl(string url)
        {
            string currentDom = CrawlerHelper.GetDomainName(url);
            if (string.IsNullOrEmpty(currentDom) || currentDom == DomainName)
            {
                if (string.IsNullOrEmpty(currentDom) && url.StartsWith("/"))
                {
                    url = "http://" + DomainName + "/www" + url;
                }
                else if (currentDom != DomainName)
                {
                    url = "http://" + DomainName + "/www/" + url;
                }
                else
                {
                    url = string.Empty;
                }
            }
            else
            {
                url = string.Empty;
            }
            return url;
        }
        #endregion

        #region 根据内容页地址获取内容信息

        protected override bool IsPageHasContent(string strWebData)
        {
            return !string.IsNullOrEmpty(strWebData) &&
                    (strWebData.Contains("<td colspan=\"2\" id='doctitle'>") ||
                     strWebData.Contains("<td colspan=\"2\" class=\"text1\" id=zoom>"));
        }
        #endregion

        #region 解析网站内容的一些方法

        protected override bool HasPagerInPage(string pageStr)
        {
            return pageStr.Contains("页/共");
        }

        protected override bool IsIgnoreUrl(string url)
        {
            return base.IsIgnoreUrl(url) && url.Contains("NewsInfo.asp?NewsId=");
        }

        protected override string GetConTitle(string dataStr)
        {
            string title = string.Empty;
            Regex regex = new Regex(TitleRegex.Expression, TitleRegex.Options);
            Match match = regex.Match(dataStr);
            if (match != null && match.Groups != null && match.Groups.Count > 0)
            {
                title = match.Groups[2].Value;
                if (string.IsNullOrEmpty(title))
                {
                    title = match.Groups[4].Value;
                }
            }

            return title;
        }

        protected override string GetPublishSource(string dataStr)
        {
            if (dataStr.Contains("<div align=\"center\">信息来源：</div>"))
            {
                return base.GetPublishSource(dataStr);
            }
            else
            {
                return InfoSource;
            }
        }

        protected override string GetContent(string dataStr)
        {
            string resultStr = string.Empty;

            if (!dataStr.Contains("<TD class=titsj id=zoom align=left>") && !dataStr.Contains("<TD class=Content1 vAlign=top align=left>"))
            {
                ContentRegex = new Regular("<td[\\s\\S]*?class=\"text1\" id=zoom>([\\s\\S]*?)</td>", DefaultRegexOptions);
                resultStr = CrawlerHelper.GetDataByRegex(dataStr, ContentRegex, false);
            }
            else
            {
                ContentRegex = new Regular("<(?<td>[\\w]+)[^>]* class=(?<Quote>[\"']?)text1(?(Quote)\\k<Quote>)"
                    + "[\"']?\\s*?id=zoom[^>]*>((?<Nested><\\k<td>[^>]*>)|</\\k<td>>(?<-Nested>)|.*?)*</\\k<td>>"
                    , RegexOptions.IgnoreCase | RegexOptions.Singleline);
                dataStr = CrawlerHelper.GetDataByRegex(dataStr, ContentRegex, true);

                if (dataStr.Contains("<TD class=titsj id=zoom align=left>"))
                {
                    ContentRegex = new Regular("<td[\\s\\S]*class=titsj id=zoom align=left>([\\s\\S]*)</td>", DefaultRegexOptions);
                    resultStr = CrawlerHelper.GetDataByRegex(dataStr, ContentRegex, false);
                }
                else if (dataStr.Contains("<TD class=Content1 vAlign=top align=left>"))
                {
                    ContentRegex = new Regular("<td[\\s\\S]*class=Content1 vAlign=top align=left>([\\s\\S]*)</td>", DefaultRegexOptions);
                    resultStr = CrawlerHelper.GetDataByRegex(dataStr, ContentRegex, false);
                }
                else
                {
                    string conPattern = "<p.[^>]*>([\\s\\S]*)</p>";
                    MatchCollection matches = new Regex(conPattern).Matches(dataStr);

                    if (matches != null && matches.Count > 0)
                    {
                        foreach (Match item in matches)
                        {
                            resultStr += item.Value + "<br/>";
                        }
                    }
                }
            }
            return resultStr;
        }

        #endregion
    }
}
