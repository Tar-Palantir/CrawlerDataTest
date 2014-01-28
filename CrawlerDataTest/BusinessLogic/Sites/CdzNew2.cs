using System;
using System.Text.RegularExpressions;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    /// <summary>
    /// 中国开发区信息网抓取类
    /// 地址：http://www.cdz.cn
    /// </summary>
    public class CdzNew2 : TestBaseCrawler
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
            TotalPageCountRegex = new Regular("<a href=\\?t=(\\d+)&page=(?<PageCount>\\d*?)>尾页</a></td>", DefaultRegexOptions);

            PageGetRegex = new Regular("&Page=(?<PageNum>\\d*?)", DefaultRegexOptions);

            HrefRegex = new Regular("<td class=\"font_14_0\">·<a href=\"(?<Url>.+?)\">(?<Title>.+?)</a></td>\\s*<td align=\"right\" class=\"font_14_0\">(?<Date>[\\s\\S]*?)</td>", DefaultRegexOptions);
        }

        protected override void ContentPageRegAndParamInitialize()
        {
            TitleRegex = new Regular("<td height=\"40\" align=\"center\" class=\"border_bottom\"><strong class=\"font_22\">([^<]+?)</strong></td>", DefaultRegexOptions);

            PublishDateRegex = new Regular("<td align=\"left\">添加时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2} \\d{1,2}:\\d{2}:\\d{2}) 来源：(?<Source>[^<]+?)</td>", DefaultRegexOptions);

            PublishSourceRegex = new Regular("<td align=\"left\">添加时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2} \\d{1,2}:\\d{2}:\\d{2}) 来源：(?<Source>[^<]+?)</td>", DefaultRegexOptions);

            ContentRegex = new Regular("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\\s*<tr>\\s*<td align=\"left\" class=\"font_14_0\">([\\S\\s]*?)</td>\\s*</tr>\\s*</table>", DefaultRegexOptions);
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
            Regex regex = new Regex("<td align=\"center\" class=\"font14_3\">共计\\d+条信息 <font color=999999>首页", DefaultRegexOptions);
            return !string.IsNullOrEmpty(strWebData) && regex.IsMatch(strWebData);
        }

        #endregion

        #region 根据内容页地址获取内容信息

        protected override bool IsPageHasContent(string strWebData)
        {
            Regex regex = new Regex("<td height=\"23\" align=\"left\">您的位置：([^\\&]*&gt; )*正文</td>", DefaultRegexOptions);
            return !string.IsNullOrEmpty(strWebData) && regex.IsMatch(strWebData);
        }
        #endregion

        #region 解析网站内容的一些方法

        protected override bool HasPagerInPage(string pageStr)
        {
            return !pageStr.Contains("<font color=999999>下一页</font>&nbsp;&nbsp;<font color=999999>尾页</font></td>");
        }

        #endregion
    }
}
