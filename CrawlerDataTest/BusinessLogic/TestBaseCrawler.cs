/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/4/1 13:09:51
 * Coder by:	陈明(Astaldo)  at 2013/4/1 13:09:51
 * Modified by:	陈明(Astaldo)  at  2013/4/1 13:09:51
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   TestBaseCrawler
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CrawlerDataTest.DataAccess.Entities;
using CrawlerDataTest.DataAccess.Repository;
using System.Text.RegularExpressions;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    public abstract class TestBaseCrawler : BaseCrawler
    {
        protected RegexOptions DefaultRegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        /// <summary>
        /// 添加抓取到的信息
        /// </summary>
        /// <param name="contentInfo"></param>
        /// <returns></returns>
        protected override void AddContentInfo(CrawlerContentInfo contentInfo, out ResultStatus status)
        {
            ContentInfo info = new ContentInfo()
            {
                Content = contentInfo.Content,
                InformationSource = contentInfo.InformationSource,
                PublishTime = contentInfo.PublishTime,
                Title = contentInfo.Title
            };

            ContentInfoRepository repository = new ContentInfoRepository();
            
            repository.Create(info, out status);
        }
    }
}