/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/3/29 10:53:17
 * Coder by:	陈明(Astaldo)  at 2013/3/29 10:53:17
 * Modified by:	陈明(Astaldo)  at  2013/3/29 10:53:17
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   GuanduGov
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CrawlerDataTest.DataAccess.Entities;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    /// <summary>
    /// 昆明官渡政府网抓取类
    /// 地址：http://gd.km.gov.cn
    /// </summary>
    public class GuanduGov : TestBaseCrawler
    {
        #region 变量定义

        /// <summary>
        /// 信息采集于昆明官渡政府网
        /// </summary>
        public override string InfoSource { get { return "昆明官渡政府网"; } }

        /// <summary>
        /// 链接的域名
        /// </summary>
        public override string DomainName { get { return "gd.km.gov.cn"; } }

        #endregion

        protected override void ListPageRegAndParamInitialize()
        {
            TotalPageCountRegex = new Regular("<div class=\"pagebox\">([\\s]*)共 (?<PageCount>\\d*) 页，"
                + "第 (\\d*) 页.*首 页.*上一页.*下一页.*尾 页.*[\\s]*</div>", DefaultRegexOptions);

            PageGetRegex = new Regular("&page=(?<PageNum>\\d*)", DefaultRegexOptions);

            HrefRegex = new Regular("<td class=\"titlelink\"><a href=\"(?<Url>.+?)\" target=\"_blank\">"
                + "(?<Title>.+?)</a></td>[\\s]*<td class=\"titlelink\" width=\"66\" nowrap=\"nowrap\"><em>"
                + "<font color=\"#666666\">(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})</font></em></td>", DefaultRegexOptions);
        }

        protected override void ContentPageRegAndParamInitialize()
        {
            TitleRegex = new Regular("<!--内容标题-->[\\s]*<h1>([\\s\\S]*?)</h1>", DefaultRegexOptions);

            PublishDateRegex = new Regular("<div class=\"info\"><!-- 来源：(?<Source>昆明官渡)"
                + " 点击：([\\s\\S]*)-->时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})</div>", DefaultRegexOptions);

            PublishSourceRegex = new Regular("<div class=\"info\"><!-- 来源：(?<Source>昆明官渡)"
                + " 点击：([\\s\\S]*)-->时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})</div>", DefaultRegexOptions);

            ContentRegex = new Regular("<div class=\"Content\" align=\"left\"><p>([\\s\\S]*?)</p></div>"
                + "[\\s]*<div class=\"Content\" align=\"left\"><ol></ol></div>", DefaultRegexOptions);
        }

        public override string Start<TOption>(Uri url, TOption option)
        {
            #region 配置爬虫选项
            option.IsListUriTransform = true;
            option.IsCrawlerByNum = true;
            option.CrawlerNum = 10;
            option.MaxCrawlerNum = 20;
            #endregion
            
            return base.Start(url, option);
        }
        
        protected override Uri ListUriTransform(Uri url)
        {
            if (url != null)
            {
                //http://gd.km.gov.cn/list.aspx?id=508322116026
                //http://gd.km.gov.cn/classlist.aspx?no-cache=0.6115979834270044&id=508322116026&page=1

                if (!string.IsNullOrEmpty(DomainName))
                {
                    if (url.AbsolutePath == "/list.aspx" && !string.IsNullOrEmpty(url.Query))
                    {
                        Uri newUri = new Uri(url.GetLeftPart(UriPartial.Authority)
                            + "/classlist.aspx?no-cache=" + new Random().NextDouble()
                            + "&" + url.Query.Substring(1) + "&page=1");
                        return newUri;
                    }
                }
            }
            return url;
        }
       
        protected override bool IsPageHasContent(string strWebData)
        {
            return !string.IsNullOrEmpty(strWebData) && strWebData.Contains("<div class=\"Content\" align=\"left\"><p>")
                    && strWebData.Contains("<div class=\"Content\" align=\"left\"><ol></ol></div>");
        }        
    }


}