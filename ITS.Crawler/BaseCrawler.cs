/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/3/29 17:04:42
 * Coder by:	陈明(Astaldo)  at 2013/3/29 17:04:42
 * Modified by:	陈明(Astaldo)  at  2013/3/29 17:04:42
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   CrawlerBaseHelper
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

namespace ITS.Crawler
{
    /// <summary>
    /// 网络爬虫基本类
    /// </summary>
    public abstract class BaseCrawler
    {
        #region 爬虫方法

        #region 变量定义

        /// <summary>
        /// 信息采集于中国开发区信息网
        /// </summary>
        public abstract string InfoSource { get; }

        /// <summary>
        /// 链接的域名
        /// </summary>
        public abstract string DomainName { get; }

        /// <summary>
        /// 缓存下来的地址
        /// </summary>
        protected Uri OraginalUrl { set; get; }

        /// <summary>
        /// 爬虫选项
        /// </summary>
        protected CrawlerOption Options { set; get; }

        /// <summary>
        /// 提取的所有内容页的链接
        /// </summary>
        protected List<string> contentUrls = new List<string>();

        private bool isKeepingCrawlerData = true;
        #endregion

        #region 启动爬虫程序

        protected virtual void InitializtionData()
        {
            //全局变量初始化
            isKeepingCrawlerData = true;
            contentUrls = new List<string>();

            ListPageRegAndParamInitialize();
            ContentPageRegAndParamInitialize();
        }

        /// <summary>
        /// 列表页面的正则表达式和参数的初始化
        /// </summary>
        protected abstract void ListPageRegAndParamInitialize();

        /// <summary>
        /// 内容页面的正则表达式和参数的初始化
        /// </summary>
        protected abstract void ContentPageRegAndParamInitialize();

        /// <summary>
        /// 异步启动爬虫程序,默认启动参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBackFun"></param>
        public void AsyncStart(string url, AsyncCallback callBackFun)
        {
            new Func<Uri, CrawlerOption, string>(Start).BeginInvoke(new Uri(url), new CrawlerOption(), callBackFun, null);
        }

        /// <summary>
        /// 启动爬虫程序,默认启动参数
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual string Start(string url)
        {
            return this.Start(new Uri(url), new CrawlerOption());
        }
        
        /// <summary>
        /// 启动爬虫程序
        /// </summary>
        /// <param name="url"></param>
        public virtual string Start<TOption>(Uri url, TOption option) where TOption : CrawlerOption
        {
            try
            {
                InitializtionData();

                if (url != null && option != null)
                {
                    this.Options = option;
                    if (option.IsListUriTransform)
                    {
                        OraginalUrl = ListUriTransform(url);
                    }
                    else
                    {
                        OraginalUrl = url;//每次启动都把输入的地址缓存下来，以供后台定时查找
                    }
                }
                else
                {
                    return "url和选项都不能为空";
                }

                //获取所有内容页地址集合
                GetContentUrls();

                //获取页面内容并保存，返回操作提示
                return GetContentInfoAndSave();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region 添加数据
        /// <summary>
        /// 添加抓取到的信息
        /// </summary>
        /// <param name="contentInfo"></param>
        /// <returns></returns>
        protected abstract void AddContentInfo(CrawlerContentInfo contentInfo, out ResultStatus status);

        #endregion

        #region 获取所有符合条件的内容页地址
        /// <summary>
        /// 判断页面是否是列表页
        /// </summary>
        /// <param name="strWebData">页面</param>
        /// <returns></returns>
        protected virtual bool IsPageHasList(string strWebData) { return true; }

        /// <summary>
        /// 获取所有符合条件的内容页地址
        /// </summary>
        /// <returns></returns>
        protected virtual void GetContentUrls()
        {
            string strWebData = string.Empty;

            if (!string.IsNullOrEmpty(OraginalUrl.AbsoluteUri))
            {
                strWebData = GetPageListHtmlData(OraginalUrl.AbsoluteUri, 1);

                //列表页
                if (IsPageHasList(strWebData))
                {
                    //获取所有内容页链接
                    contentUrls = GetContentHrefs(strWebData, OraginalUrl.AbsoluteUri);
                }
            }

            contentUrls = contentUrls.Distinct().ToList();

            //该网址不是列表页地址
            if (null == contentUrls || contentUrls.Count == 0)
            {
                contentUrls.Add(OraginalUrl.AbsoluteUri);
            }
        }
        #endregion

        #region 根据内容页地址获取内容信息
        /// <summary>
        /// 切换选项和正则表达式
        /// </summary>
        /// <param name="url"></param>
        protected virtual void SwitchOptionAndRegex(string url) { }

        /// <summary>
        /// 判断页面是否是内容页面
        /// </summary>
        /// <param name="strWebData">页面</param>
        /// <returns></returns>
        protected virtual bool IsPageHasContent(string strWebData) { return true; }

        /// <summary>
        /// 根据内容页地址获取内容信息
        /// </summary>
        /// <param name="contentUrl"></param>
        /// <returns></returns>
        protected virtual CrawlerContentInfo GetContentInfo(string contentUrl)
        {
            string strWebData = string.Empty;

            CrawlerContentInfo contentInfo = new CrawlerContentInfo();

            if (!string.IsNullOrEmpty(contentUrl))
            {
                SwitchOptionAndRegex(contentUrl);

                Uri newUri = new Uri(contentUrl);
                if (Options.IsContentUriTransform)
                {
                    newUri = ContentUriTransform(newUri);
                }
                strWebData = GetContentHtmlData(newUri.AbsoluteUri, 1);

                if (IsPageHasContent(strWebData))
                {
                    //标题
                    var title = GetConTitle(strWebData);

                    //发布时间
                    var publishDate = GetPublishDate(strWebData);

                    //来源
                    var source = GetPublishSource(strWebData);

                    //内容
                    string content = string.Empty;
                    if (!string.IsNullOrEmpty(title))
                    {
                        content = GetContentAll(strWebData, contentUrl);
                    }

                    contentInfo.Title = title;
                    DateTime conDate = DateTime.MinValue;
                    DateTime.TryParse(publishDate, out conDate);
                    contentInfo.PublishTime = conDate;
                    contentInfo.InformationSource = source;
                    contentInfo.Content = content;
                }
                else
                {
                    contentInfo = null;
                }
            }
            else
            {
                contentInfo = null;
            }
            return contentInfo;
        }

        /// <summary>
        /// 获取内容html原文件内容
        /// </summary>
        /// <param name="contentUrl"></param>
        /// <returns></returns>
        protected virtual string GetContentHtmlData(string contentUrl, int pageNum)
        {
            if (Options.ContentParamMethod == ParamTransMethod.Post)
            {
                return GetPageDataByPost(contentUrl, pageNum, this.ContentPostParam);
            }
            else if (Options.ContentParamMethod == ParamTransMethod.File)
            {
                return GetPageDataByFile(contentUrl, pageNum, this.ContentFileRegex);
            }
            else if (Options.ContentParamMethod == ParamTransMethod.Get)
            {
                return GetPageDataByGet(contentUrl, pageNum, this.ContentGetRegex);
            }
            else//Dymanic
            {
                return GetPageDataByDymanic(contentUrl, pageNum, this.ContentGetRegex, this.ContentFileRegex, this.ContentPostParam);
            }
        }
        #endregion

        #region Uri转换
        /// <summary>
        /// 将链接地址转换成采集地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual Uri ListUriTransform(Uri url)
        {
            return url;
        }

        /// <summary>
        /// 将链接地址转换成采集地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual Uri ContentUriTransform(Uri url)
        {
            return url;
        }

        /// <summary>
        /// Url转换为绝对Url地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual string UrlToAbsUrl(string url)
        {
            string currentDom = CrawlerHelper.GetDomainName(url);
            if (string.IsNullOrEmpty(currentDom) || currentDom == DomainName)
            {
                if (string.IsNullOrEmpty(currentDom) && url.StartsWith("/"))
                {
                    url = "http://" + DomainName + url;
                }
                else if (currentDom != DomainName)
                {
                    url = "http://" + DomainName + "/" + url;
                }
            }
            else
            {
                url = string.Empty;
            }
            return url;
        }

        #endregion

        #region 获取Url
        /// <summary>
        /// 从列表页获取所有内容页的链接（包括所有分页中的信息）
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="currentUrl"></param>
        /// <returns></returns>
        protected virtual List<string> GetContentHrefs(string dataStr, string currentUrl)
        {
            try
            {
                //包含分页
                if (HasPagerInPage(dataStr))
                {
                    int pageNum = 1;

                    int totalPageCount = GetTotalPageCount(dataStr, this.TotalPageCountRegex);
                    int maxPageNum = totalPageCount > 50 ? 50 : totalPageCount;

                    string strWebData = dataStr;
                    while (isKeepingCrawlerData)
                    {
                        var tempChilds = GetCurrentConHrefs(strWebData);

                        if (tempChilds != null && tempChilds.Count > 0)
                        {
                            contentUrls.AddRange(tempChilds);
                        }
                        else
                        {
                            break;
                        }

                        if (pageNum >= maxPageNum)
                        {
                            break;
                        }
                        pageNum++;

                        string pageUrl = currentUrl;

                        strWebData = GetPageListHtmlData(currentUrl, pageNum);

                    }
                }
                else
                {
                    var tempChilds = GetCurrentConHrefs(dataStr);

                    if (tempChilds != null && tempChilds.Count > 0)
                    {
                        contentUrls.AddRange(tempChilds);
                    }
                }

                return contentUrls.Distinct().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取列表页面的源代码
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        protected virtual string GetPageListHtmlData(string pageUrl, int pageNum)
        {

            if (Options.ListParamMethod == ParamTransMethod.Post)
            {
                return GetPageDataByPost(pageUrl, pageNum, this.PagePostParam);
            }
            else if (Options.ListParamMethod == ParamTransMethod.File)
            {
                return GetPageDataByFile(pageUrl, pageNum, this.PageFileRegex);
            }
            else if (Options.ListParamMethod == ParamTransMethod.Get)
            {
                return GetPageDataByGet(pageUrl, pageNum, this.PageGetRegex);
            }
            else//Dymanic
            {
                return GetPageDataByDymanic(pageUrl, pageNum, this.PageGetRegex, this.PageFileRegex, this.PagePostParam);
            }
        }


        /// <summary>
        /// 判断是否忽略该链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual bool IsIgnoreUrl(string url)
        {
            return !url.Contains(".docx") && !url.Contains(".doc") && !url.Contains(".txt") && !url.Contains(".ico")
                        && !url.Contains(".psd") && !url.Contains(".png") && !url.Contains(".gif") && !url.Contains(".jpg")
                        && !url.Contains(".css") && !url.Contains(".js") && !url.Contains("javascript:")
                        && !contentUrls.Contains(url);
        }

        /// <summary>
        /// 获取列表当前页中所有内容页的链接
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="currentUrl"></param>
        /// <returns></returns>
        protected virtual List<string> GetCurrentConHrefs(string dataStr)
        {
            List<string> childs = new List<string>();
            Regex reg = new Regex(this.HrefRegex.Expression, this.HrefRegex.Options);
            MatchCollection matches = reg.Matches(dataStr);
            //将当前列表页中的内容页链接加入到集合中
            if (matches != null && matches.Count > 0)
            {
                foreach (Match item in matches)
                {
                    string url = item.Groups["Url"].ToString();
                    //string title = item.Groups["Title"].ToString();
                    //发布时间
                    string date = item.Groups["Date"].ToString();

                    if (IsIgnoreUrl(url))
                    {
                        url = UrlToAbsUrl(url);

                        if (!childs.Contains(url) && !string.IsNullOrEmpty(url))
                        {
                            if (IsKeepingCrawlerData(date, childs.Count))
                            {
                                childs.Add(url);
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                }
            }
            return childs;
        }

        /// <summary>
        /// 判断采集是否继续
        /// </summary>
        /// <param name="dataDate"></param>
        /// <returns></returns>
        protected virtual bool IsKeepingCrawlerData(string dataDate, int thisPageCurrentCount)
        {
            if (Options.IsCrawlerByNum)
            {
                isKeepingCrawlerData = contentUrls.Count + thisPageCurrentCount < Options.CrawlerNum && Options.CrawlerNum < Options.MaxCrawlerNum;

                return isKeepingCrawlerData;
            }
            else
            {
                DateTime defDate = DateTime.MinValue;
                DateTime.TryParse(dataDate, out defDate);

                int days = Options.CrawlerDays;
                int maxDays = Options.MaxCrawlerDays;
                return defDate >= Convert.ToDateTime(DateTime.Now.AddDays(-days).ToShortDateString())
                                    && defDate <= Convert.ToDateTime(DateTime.Now.AddDays(-maxDays).ToShortDateString());
            }
        }
        #endregion

        #region 分页信息

        /// <summary>
        /// 判断列表页面中是否有分页
        /// </summary>
        /// <param name="pageStr">页面</param>
        /// <returns></returns>
        protected virtual bool HasPagerInPage(string pageStr) { return true; }

        /// <summary>
        /// 获取列表页总页数
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        protected virtual int GetTotalPageCount(string dataStr, Regular regular)
        {
            int totalPageCount = 50;
            Regex reg = new Regex(regular.Expression, regular.Options);
            MatchCollection matches = reg.Matches(dataStr);
            if (matches != null && matches.Count > 0)
            {
                int.TryParse(matches[matches.Count - 1].Groups["PageCount"].Value, out totalPageCount);
            }
            return totalPageCount;
        }

        /// <summary>
        /// Get方式获取指定页码的页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageNum"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        protected virtual string GetPageDataByPost(string url, int pageNum, string paramStr)
        {
            paramStr = Regex.Replace(paramStr, @"\(\?\<PageNum\>.[^)]*\)", pageNum.ToString());

            return CrawlerHelper.GetHtmlData(url, paramStr);
        }

        /// <summary>
        /// 文件名的方式获取指定页码的页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageNum"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        protected virtual string GetPageDataByFile(string url, int pageNum, Regular regular)
        {
            string strPageNums = CrawlerHelper.GetDataByRegex(url, regular, false);

            if (strPageNums.Contains("_"))
            {
                string regStr = "_\\d*";
                Regex regex = new Regex(regStr, regular.Options);
                if (1 == pageNum)
                {
                    url = regex.Replace(url, "");
                }
                else
                {
                    url = regex.Replace(url, "_" + pageNum.ToString());
                }
            }
            else
            {
                if (1 != pageNum)
                {
                    url = url.Replace(strPageNums, strPageNums + "_" + pageNum.ToString());
                }
            }
            return CrawlerHelper.GetHtmlData(url);
        }

        /// <summary>
        /// Post方式获取指定页码的页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageNum"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        protected virtual string GetPageDataByGet(string url, int pageNum, Regular regular)
        {
            string regStr = regular.Expression;
            string replaceStr = Regex.Replace(regStr, @"\(\?\<PageNum\>.[^)]*\)", pageNum.ToString());

            Regex regex = new Regex(regStr, regular.Options);
            if (regex.IsMatch(url))
            {
                url = regex.Replace(url, replaceStr);
            }
            else
            {
                if (url.Contains("?"))
                {
                    url += "&" + replaceStr;
                }
                else
                {
                    url += "?" + replaceStr;
                }
            }
            return CrawlerHelper.GetHtmlData(url);
        }

        /// <summary>
        /// 动态方式获取指定页码的页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageNum"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        protected virtual string GetPageDataByDymanic(string url, int pageNum, Regular getRegular = null, Regular fileRegular = null, string postParamStr = null)
        {
            if (getRegular == null && fileRegular == null && string.IsNullOrEmpty(postParamStr))
            {
                return string.Empty;
            }

            string strWebData = this.GetPageDataByGet(url, pageNum, getRegular);
            if (string.IsNullOrEmpty(strWebData))
            {
                strWebData = this.GetPageDataByFile(url, pageNum, fileRegular);
                if (string.IsNullOrEmpty(strWebData))
                {
                    strWebData = this.GetPageDataByPost(url, pageNum, postParamStr);
                }
            }
            return strWebData;
        }

        /// <summary>
        /// 判断内容页面中是否有分页
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        protected virtual bool HasPagerInContent(string strData) { return false; }

        #endregion

        #region 获取内容等的正则表达式

        /// <summary>
        /// 列表页总页数表达式(PageCount)
        /// </summary>
        public virtual Regular TotalPageCountRegex
        {
            set { _totalPageCountRegex = value; }
            get
            {
                if (_totalPageCountRegex != null) return _totalPageCountRegex;
                else
                {
                    return new Regular("<div class=\"pagebox\">([\\s]*)共 (?<PageCount>\\d*) 页，"
                        + "第 (\\d*) 页.*首 页.*上一页.*下一页.*尾 页.*[\\s]*</div>",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _totalPageCountRegex;

        /// <summary>
        /// 页面正则表达式
        /// </summary>
        public virtual Regular PageGetRegex
        {
            set { _pageGetRegex = value; }
            get
            {
                if (_pageGetRegex != null) return _pageGetRegex;
                else
                {
                    return new Regular("currentPageNum=(?<PageNum>\\d*)",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _pageGetRegex;

        /// <summary>
        /// 页面正则表达式
        /// </summary>
        public virtual Regular PageFileRegex
        {
            set { _pageFileRegex = value; }
            get
            {
                if (_pageFileRegex != null) return _pageFileRegex;
                else
                {
                    return new Regular("[\\s\\S]*?/([\\w]*?).htm",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _pageFileRegex;

        /// <summary>
        /// 页面Post参数
        /// </summary>
        public virtual string PagePostParam
        {
            set { _pagePostParam = value; }
            get
            {
                if (_pagePostParam != null) return _pagePostParam;
                else
                {
                    return "{\"id\": 2, \"method\": \"BrowserRPC.getDocumentListByID\", \"params\": [\"(?<DepartmentId>\\d*)\", \"(?<ClassId>\\d*)\", \"(?<PageNum>\\d*)\", \"30\"]}";
                }
            }
        }
        protected string _pagePostParam;

        /// <summary>
        /// 内容页面的总页数表达式(PageCount)
        /// </summary>
        public virtual Regular TotalContentCountRegex
        {
            set { _totalContentCountRegex = value; }
            get
            {
                if (_totalContentCountRegex != null) return _totalContentCountRegex;
                else
                {
                    return new Regular("<A class=\"turnpage\" HREF=\"(.+?)\">(?<PageCount>.+?)</A>",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _totalContentCountRegex;

        /// <summary>
        /// 内容页面正则表达式或者Post参数
        /// </summary>
        public virtual Regular ContentGetRegex
        {
            set { _contentGetRegex = value; }
            get
            {
                if (_contentGetRegex != null) return _contentGetRegex;
                else
                {
                    return new Regular("currentPageNum=(?<PageNum>\\d*)",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _contentGetRegex;

        /// <summary>
        /// 内容Post参数
        /// </summary>
        public virtual string ContentPostParam
        {
            set { _contentPostParam = value; }
            get
            {
                if (_contentPostParam != null) return _contentPostParam;
                else
                {
                    return "{\"id\": 4, \"method\": \"BrowserRPC.getDocumentInfoByID\", \"params\": [\"(?<DocumentId>\\d*)\"]}";
                }
            }
        }
        protected string _contentPostParam;

        /// <summary>
        /// 内容页面正则表达式或者Post参数
        /// </summary>
        public virtual Regular ContentFileRegex
        {
            set { _contentFileRegex = value; }
            get
            {
                if (_contentFileRegex != null) return _contentFileRegex;
                else
                {
                    return new Regular("currentPageNum=\\d*",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _contentFileRegex;

        /// <summary>
        /// 获取内容链接正则表达式，链接(Url)，标题(Title)，时间(Date)
        /// </summary>
        public virtual Regular HrefRegex
        {
            set { _hrefRegex = value; }
            get
            {
                if (_hrefRegex != null) return _hrefRegex;
                else
                {
                    return new Regular("<a href=\"(?<Url>.+?)\"\\s\\S*title=\"(?<Title>.+?)\"\\s\\S*>[\\s\\S]*?(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})[\\s\\S]*?",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _hrefRegex;

        /// <summary>
        /// 获取内容标题正则表达式
        /// </summary>
        public virtual Regular TitleRegex
        {
            set { _titleRegex = value; }
            get
            {
                if (_titleRegex != null) return _titleRegex;
                else
                {
                    return new Regular("<div id=\"title\">([\\s\\S]*?)</div>",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _titleRegex;

        /// <summary>
        /// 获取子标题正则表达式
        /// </summary>
        public virtual Regular SubTitleRegex
        {
            set { _subTitleRegex = value; }
            get
            {
                if (_subTitleRegex != null) return _subTitleRegex;
                else
                {
                    return new Regular("<td class=\"subtitle\">([\\s\\S]*?)</td>",
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _subTitleRegex;

        /// <summary>
        /// 获取发布时间正则表达式(Date)
        /// </summary>
        public virtual Regular PublishDateRegex
        {
            set { _publishDateRegex = value; }
            get
            {
                if (_publishDateRegex != null) return _publishDateRegex;
                else
                {
                    return new Regular("发布时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})[\\s\\S]*)",
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _publishDateRegex;

        /// <summary>
        /// 获取发布来源正则表达式(Source)
        /// </summary>
        public virtual Regular PublishSourceRegex
        {
            set { _publishSourceRegex = value; }
            get
            {
                if (_publishSourceRegex != null) return _publishSourceRegex;
                else
                {
                    return new Regular("来源：(?<Source>[\\s\\S]*)",
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _publishSourceRegex;

        /// <summary>
        /// 获取内容正则表达式
        /// </summary>
        public virtual Regular ContentRegex
        {
            set { _contentRegex = value; }
            get
            {
                if (_contentRegex != null) return _contentRegex;
                else
                {
                    return new Regular("<div id=\"content\">([\\s\\S]*?)</div>",
                             RegexOptions.Multiline | RegexOptions.IgnoreCase);
                }
            }
        }
        protected Regular _contentRegex;

        #endregion

        #region 获取内容页面内的信息

        /// <summary>
        /// 获取页面内容并保存，返回操作提示
        /// </summary>
        /// <returns></returns>
        protected virtual string GetContentInfoAndSave()
        {
            if (contentUrls != null && contentUrls.Count > 0)
            {
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < contentUrls.Count; i++)
                {
                    //获取内容信息
                    CrawlerContentInfo contentInfo = GetContentInfo(contentUrls[i]);
                    //写入文件或数据库
                    if (null != contentInfo && !string.IsNullOrEmpty(contentInfo.Title) && !string.IsNullOrEmpty(contentInfo.Content))
                    {
                        string returnTitle = string.Empty;
                        ResultStatus status;
                        AddContentInfo(contentInfo, out status);

                        if (status.ResultSign == CrawlerResultSign.Success)
                        {//存入数据库成功
                            strBuilder.AppendLine(string.Format("抓取到{0}的第{1}条信息。。。,时间：{2}，该数据抓取成功，开始抓取下条数据，请继续等待。。。",
                                OraginalUrl.AbsoluteUri, i + 1, DateTime.Now.ToString()));
                        }
                        else if (status.ResultSign == CrawlerResultSign.Failed)
                        {//存入数据库失败
                            strBuilder.AppendLine(string.Format("抓取到{0}的第{1}条信息。。。,时间：{2}，存入数据库失败，{3}，开始抓取下条数据，请继续等待。。。",
                                OraginalUrl.AbsoluteUri, i + 1, DateTime.Now.ToString(), status.Message));
                        }
                        else
                        {//已存在数据
                            strBuilder.AppendLine(string.Format("抓取到{0}的第{1}条信息。。。,时间：{2}，已存在该数据，{3}，开始抓取下条数据，请继续等待。。。",
                                OraginalUrl.AbsoluteUri, i + 1, DateTime.Now.ToString(), status.Message));
                        }
                    }
                    else
                    {
                        strBuilder.AppendLine(string.Format("{0}无内容可以爬取。。。,时间：{1}，该数据抓取失败，开始抓取下条数据，请继续等待。。。",
                                contentUrls[i], DateTime.Now.ToString()));
                    }
                }
                return strBuilder.ToString();
            }
            return string.Format("{0}无数据需要爬取，开始抓取下个地址，请继续等待。。。", OraginalUrl.AbsoluteUri);
        }

        /// <summary>
        /// 判断是否有子标题需要采集
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        protected virtual bool HasSubTitle(string dataStr) { return false; }

        /// <summary>
        /// 获取Title
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        protected virtual string GetConTitle(string dataStr)
        {
            Regular titleRegular = this.TitleRegex;

            string title = CrawlerHelper.GetDataByRegex(dataStr, titleRegular, false);

            if (HasSubTitle(dataStr))
            {
                Regular subRegular = this.SubTitleRegex;
                string subTitle = CrawlerHelper.GetDataByRegex(dataStr, subRegular, false);

                title = title + "<br/>" + subTitle;
            }

            return title;
        }

        /// <summary>
        /// 获取发布时间
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        protected virtual string GetPublishDate(string dataStr)
        {
            string result = string.Empty;
            Regular regular = this.PublishDateRegex;

            Match match = Regex.Match(dataStr, regular.Expression, regular.Options);

            if (match != null && match.Groups != null && match.Groups.Count > 0)
            {
                string publishDate = match.Groups["Date"].Value;

                result = !string.IsNullOrEmpty(publishDate) ? publishDate : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            }

            return result;
        }

        /// <summary>
        /// 获取发布时间
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        protected virtual string GetPublishSource(string dataStr)
        {
            string result = string.Empty;
            Regular regular = this.PublishSourceRegex;

            Match match = Regex.Match(dataStr, regular.Expression, regular.Options);

            if (match != null && match.Groups != null && match.Groups.Count > 0)
            {
                string source = match.Groups["Source"].Value;

                result = !string.IsNullOrEmpty(source) ? source : InfoSource;
            }

            return result;
        }

        /// <summary>
        /// 获取内容信息
        /// </summary>
        /// <param name="strWebData"></param>
        /// <returns></returns>
        protected virtual string GetContentAll(string strWebData, string contentUrl)
        {
            string content = string.Empty;
            //如果内容页包含分页信息
            if (HasPagerInContent(strWebData))
            {
                int pageNum = GetTotalPageCount(strWebData, this.TotalContentCountRegex);

                //获取第一页的信息
                content += GetContent(strWebData);

                for (int i = 2; i <= pageNum; i++)
                {
                    string pageData = GetContentHtmlData(contentUrl, i);

                    //分页之间信息换行
                    content += "<br/>" + GetContent(pageData);
                }

            }
            else
            {
                content = GetContent(strWebData);
            }

            if (Options.IsTransContentImgUrl)
            {
                //如果内容中有图片信息的话，将图片相对路径转化为绝对路径
                if (!string.IsNullOrEmpty(content) && content.Contains("<img"))
                {
                    //获取内容中所有src
                    Regex reg = new Regex(@"(?is)<img(?:(?!src=).)*src=(['""]?)(?<url>[^""\s>]*)\1[^>]*>");
                    MatchCollection matches = reg.Matches(content);
                    if (matches != null && matches.Count > 0)
                    {
                        foreach (Match m in matches)
                        {
                            string src = m.Groups["url"].Value;
                            if (!src.Contains("http://"))
                            {
                                string tempSrc = "http://" + DomainName + src;
                                content = Regex.Replace(content, src, tempSrc);
                            }
                        }
                    }
                }
            }

            return content;
        }

        /// <summary>
        /// 获取内容信息（单页信息）
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        protected virtual string GetContent(string dataStr)
        {
            string resultStr = string.Empty;

            Regular regular = this.ContentRegex;

            resultStr = CrawlerHelper.GetDataByRegex(dataStr, regular, false);

            return resultStr;
        }
        #endregion

        #endregion
    }
}