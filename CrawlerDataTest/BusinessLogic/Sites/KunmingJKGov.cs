/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/4/8 15:34:14
 * Coder by:	陈明(Astaldo)  at 2013/4/8 15:34:14
 * Modified by:	陈明(Astaldo)  at  2013/4/8 15:34:14
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   KunmingJKGov
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    public class KunmingJKGov : TestBaseCrawler
    {

        public override string InfoSource
        {
            get { return "昆明经开区政企互动网"; }
        }

        public override string DomainName
        {
            get { return "jkpt.ketdz.gov.cn"; }
        }

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

            //PublishDateAndSourceRegex = new Regular("<div class=\"info\"><!-- 来源：(?<Source>昆明官渡)"
            //    + " 点击：([\\s\\S]*)-->时间：(?<Date>\\d{4}-\\d{1,2}-\\d{1,2})</div>", DefaultRegexOptions);

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
    }
}