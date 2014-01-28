/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/4/1 10:07:01
 * Coder by:	陈明(Astaldo)  at 2013/4/1 10:07:01
 * Modified by:	陈明(Astaldo)  at  2013/4/1 10:07:01
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   CrawlerHelperFactory
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    /// <summary>
    /// 
    /// </summary>
    public class CrawlerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private IDictionary<string, BaseCrawler> crawlerHelpers = new Dictionary<string, BaseCrawler>();

        /// <summary>
        /// 
        /// </summary>
        private static IDictionary<string, Type> crawlerTypes = new Dictionary<string, Type>();

        /// <summary>
        /// 
        /// </summary>
        static CrawlerFactory()
        {
            var subCrawlerTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(p => p.IsSubclassOf(typeof(BaseCrawler)) && !p.IsAbstract)
                .Where(p=>p.IsPublic);
            foreach (var subCrawlerType in subCrawlerTypes)
            {
                crawlerTypes.Add(((BaseCrawler)Activator.CreateInstance(subCrawlerType)).DomainName, subCrawlerType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BaseCrawler GetCrawlerHelper(string url)
        {
            string domainName = CrawlerHelper.GetDomainName(url);

            if (crawlerTypes[domainName] != null)
            {
                if (!crawlerHelpers.Keys.Contains(domainName))
                {
                    crawlerHelpers.Add(domainName, (BaseCrawler)Activator.CreateInstance(crawlerTypes[domainName]));
                }
                return crawlerHelpers[domainName];
            }
            else
            {
                return null;
            }
        }
    }
}