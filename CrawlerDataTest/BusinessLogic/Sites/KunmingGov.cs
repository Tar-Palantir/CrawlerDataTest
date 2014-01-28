using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using CrawlerDataTest.DataAccess.Entities;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    /// <summary>
    /// 中国昆明政府网抓取类
    /// 地址：http://www.km.gov.cn
    /// </summary>
    public class KunmingGov : TestBaseCrawler
    {
        #region 变量定义

        /// <summary>
        /// 信息采集于中国昆明政府网
        /// </summary>
        public override string InfoSource { get { return "中国昆明政府网"; } }

        /// <summary>
        /// 链接的域名
        /// </summary>
        public override string DomainName { get { return "www.km.gov.cn"; } }

        private bool isJspPage = false;

        private string departmentId = string.Empty;

        private string classId = string.Empty;

        private string documentId = string.Empty;

        #endregion

        protected override void ListPageRegAndParamInitialize()
        {
            if (!isJspPage)
            {
                PageFileRegex = new Regular("[\\s\\S]*?/([\\w]*?).htm", DefaultRegexOptions);//@"currentPageNum=\d*"

                HrefRegex = new Regular("<a href=\"(?<Url>.+?)\" target=\"_blank\" title=\"(?<Title>.+?)\""
                    + " class=\"(.+?)\">[\\s\\S]*?</a>\\((?<Date>\\d{4}-\\d{1,2}-\\d{1,2})\\)</td>", DefaultRegexOptions);

                TotalPageCountRegex = null;
                PagePostParam = "";
            }
            else
            {
                TotalPageCountRegex = new Regular("{\"list_count\":\"(?<PageCount>\\d*)\".*}", DefaultRegexOptions);

                PagePostParam = "{\"id\": 2, \"method\": \"BrowserRPC.getDocumentListByClassID\", \"params\": [\"(?<DepartmentId>\\d*)\", \"(?<ClassId>\\d*)\", \"(?<PageNum>\\d*)\", \"30\"]}";

                HrefRegex = new Regular("{\"documentContent\":[^}]*,\"postTime\":\"(?<Date>\\d{4}-\\d{1,2}-\\d{1,2} \\d{1,2}:\\d{2}:\\d{2})\",[^}]*,\"documentID\":\"(?<Url>\\d*)\".[^}]*}", DefaultRegexOptions);

                PageFileRegex = null;
            }
        }

        protected override void ContentPageRegAndParamInitialize()
        {
            if (!isJspPage)
            {
                TotalContentCountRegex = new Regular("<A class=\"turnpage\" HREF=\"(.+?)\">\\s{0,2}(?<PageCount>\\d*)</A>", DefaultRegexOptions);

                ContentGetRegex = new Regular("currentPageNum=(?<PageNum>\\d*)", DefaultRegexOptions);

                TitleRegex = new Regular("<div id=\"title\">([\\s\\S]*?)</div>", DefaultRegexOptions);

                SubTitleRegex = new Regular("<td class=\"CicroR3Z5P2_6599_2688_km_25_news_subtitle\">([\\s\\S]*?)</td>", DefaultRegexOptions);

                PublishDateRegex = new Regular("[\\s\\S]*?发布时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})"
                    + "[\\s\\S]*?来源：(?<Source>[\\s\\S]*?)&nbsp;", DefaultRegexOptions);

                PublishSourceRegex = new Regular("[\\s\\S]*?发布时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})"
                    + "[\\s\\S]*?来源：(?<Source>[\\s\\S]*?)&nbsp;", DefaultRegexOptions);

                ContentRegex = new Regular("<div id=\"content\">([\\s\\S]*?)</div>", DefaultRegexOptions);

                ContentPostParam = "";
            }
            else
            {
                ContentPostParam = "{\"id\": 4, \"method\": \"BrowserRPC.getDocumentInfoByID\",\"params\": [\"(?<DocumentId>\\d*)\"]}";

                TitleRegex = new Regular("{.*\"documentTitle\":\"(?<Title>[\\a-z\\d]*)\"}", DefaultRegexOptions);

                PublishDateRegex = new Regular("{.*\"postTime\":\"(?<Date>\\d{4}-\\d{1,2}-\\d{1,2} \\d{1,2}:\\d{2}:\\d{2})\".*}(?<Source>.*)", DefaultRegexOptions);

                PublishSourceRegex = new Regular("{.*\"postTime\":\"(?<Date>\\d{4}-\\d{1,2}-\\d{1,2} \\d{1,2}:\\d{2}:\\d{2})\".*}(?<Source>.*)", DefaultRegexOptions);

                ContentRegex = new Regular("{\"documentContent\":\"(?<Document>[^}]*)\",\"indexCode\":.*\"}", DefaultRegexOptions);

                TotalContentCountRegex = null;
                ContentGetRegex = null;
            }
        }

        protected override void InitializtionData()
        {
            departmentId = string.Empty;
            classId = string.Empty;
            documentId = string.Empty;

            base.InitializtionData();
        }

        /// <summary>
        /// 启动爬虫程序
        /// </summary>
        /// <param name="url"></param>
        public override string Start<TOption>(Uri url, TOption option)
        {
            isJspPage = url.AbsoluteUri.StartsWith("http://www.km.gov.cn/jsp/zwgkClient/classList.html", StringComparison.InvariantCultureIgnoreCase);

            #region 配置爬虫选项
            option.IsCrawlerByNum = true;
            option.CrawlerNum = 10;
            option.MaxCrawlerNum = 20;

            if (!isJspPage)
            {
                option.ListParamMethod = ParamTransMethod.File;
            }
            else
            {
                option.IsListUriTransform = true;
                option.IsContentUriTransform = true;
                option.ListParamMethod = ParamTransMethod.Post;
                option.ContentParamMethod = ParamTransMethod.Post;
            }
            #endregion

            return base.Start(url, option);
        }

        protected override void SwitchOptionAndRegex(string url)
        {
            bool isJspContentPage = url.StartsWith("http://www.km.gov.cn/jsp/zwgkClient/infoList.html", StringComparison.InvariantCultureIgnoreCase);
            if (isJspPage != isJspContentPage)
            {
                isJspPage = isJspContentPage;
                if (!isJspPage)
                {
                    Options.IsContentUriTransform = false;
                    Options.ContentParamMethod = ParamTransMethod.Get;
                }
                else
                {
                    Options.IsContentUriTransform = true;
                    Options.ContentParamMethod = ParamTransMethod.Post;
                }
                ContentPageRegAndParamInitialize();
            }
        }

        protected override bool IsPageHasList(string strWebData)
        {
            if (!isJspPage)
            {
                return !string.IsNullOrEmpty(strWebData) && strWebData.Contains("<INPUT TYPE=\"button\" Value=\"GO\" OnClick=\"fnGoPage(document.all.currentPageNum.value)\"");
            }
            else
            {
                return true;
            }
        }

        protected override bool IsPageHasContent(string strWebData)
        {
            if (!isJspPage)
            {
                return !string.IsNullOrEmpty(strWebData) && strWebData.Contains("<div id=\"CicroAONI57_7832_7258_km_25__content\">")
                        && strWebData.Contains("<div id=\"content\">");
            }
            else
            {
                return true;
            }
        }

        protected override Uri ListUriTransform(Uri url)
        {
            if (!isJspPage)
            {
                return url;
            }
            else
            {
                //http://www.km.gov.cn/jsp/zwgkClient/classList.html?departmentid=8981&classid=117364
                Regex regex = new Regex("departmentid=(?<DepartmentId>\\d*)&classid=(?<ClassId>\\d*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match match = regex.Match(url.Query);

                departmentId = match.Groups["DepartmentId"].Value;
                classId = match.Groups["ClassId"].Value;

                PagePostParam = Regex.Replace(Regex.Replace(PagePostParam, @"\(\?\<DepartmentId\>.[^)]*\)", departmentId)
                    , @"\(\?\<ClassId\>.[^)]*\)", classId);


                return new Uri("http://www.km.gov.cn/JSON-RPC");
            }
        }

        protected override Uri ContentUriTransform(Uri url)
        {
            if (!isJspPage)
            {
                return url;
            }
            else
            {
                bool isJspContentPage = url.AbsoluteUri.StartsWith("http://www.km.gov.cn/jsp/zwgkClient/infoList.html", StringComparison.InvariantCultureIgnoreCase);
                if (isJspContentPage)
                {
                    Regex regex = new Regex("departmentid=(?<DepartmentId>\\d*)&classid=(?<ClassId>\\d*)&documentid=(?<DocumentId>\\d*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Match match = regex.Match(url.Query);

                    departmentId = match.Groups["DepartmentId"].Value;
                    classId = match.Groups["ClassId"].Value;
                    documentId = match.Groups["DocumentId"].Value;
                }
                else
                {
                    documentId = url.AbsolutePath.Substring(1);
                }
                return new Uri("http://www.km.gov.cn/JSON-RPC");
            }
        }

        protected override bool HasPagerInPage(string pageStr)
        {
            if (!isJspPage)
            {
                return pageStr.Contains("<INPUT TYPE=\"button\" Value=\"GO\" OnClick=\"fnGoPage(document.all.currentPageNum.value)\"");
            }
            else
            {
                return true;
            }
        }
        
        protected override bool HasSubTitle(string dataStr)
        {
            if (!isJspPage)
            {
                return dataStr.Contains("class=\"CicroR3Z5P2_6599_2688_km_25_news_subtitle\"");
            }
            else
            {
                return false;
            }
        }

        protected override bool HasPagerInContent(string strData)
        {
            if (!isJspPage)
            {
                return strData.Contains("<tr class=\"page\">");
            }
            else
            {
                return false;
            }
        }

        protected override List<string> GetCurrentConHrefs(string dataStr)
        {
            if (!isJspPage)
            {
                int index = dataStr.IndexOf("<td width=\"699\" align=\"left\" vAlign=\"top\">", StringComparison.CurrentCultureIgnoreCase);
                dataStr = dataStr.Substring(index < 0 ? 0 : index);
            }
            return base.GetCurrentConHrefs(dataStr);
        }

        protected override string GetConTitle(string dataStr)
        {
            string title = base.GetConTitle(dataStr);
            if (isJspPage)
            {
                title = CrawlerHelper.UnicodeToString(title);
            }
            return title;
        }

        protected override string GetContentAll(string strWebData, string contentUrl)
        {
            string content = base.GetContentAll(strWebData, contentUrl);
            if (isJspPage)
            {
                content = CrawlerHelper.UnicodeToString(content);
            }
            return content;
        }

        protected override string GetPageDataByPost(string url, int pageNum, string paramStr)
        {
            if (isJspPage)
            {
                pageNum = (pageNum - 1) * 30;
                paramStr = Regex.Replace(paramStr, @"\(\?\<DocumentId\>.[^)]*\)", documentId);
            }
            return base.GetPageDataByPost(url, pageNum, paramStr);
        }

        protected override int GetTotalPageCount(string dataStr, Regular regular)
        {
            int pageCount = base.GetTotalPageCount(dataStr, regular);
            if (isJspPage)
            {
                pageCount = Convert.ToInt32(Math.Ceiling((double)pageCount / 30));
            }
            return pageCount;
        }
    }
}
