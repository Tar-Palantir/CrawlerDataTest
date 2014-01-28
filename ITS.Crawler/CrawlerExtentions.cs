/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/4/1 10:43:36
 * Coder by:	陈明(Astaldo)  at 2013/4/1 10:43:36
 * Modified by:	陈明(Astaldo)  at  2013/4/1 10:43:36
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   CrawlerContentInfo
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ITS.Crawler
{
    public class CrawlerContentInfo
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string InformationSource { get; set; }

        public System.DateTime PublishTime { get; set; }
    }

    public class ResultStatus
    {
        public CrawlerResultSign ResultSign { set; get; }

        public string Message { set; get; }
    }

    public class Regular
    {
        public Regular(string expression)
        {
            this.Expression = expression;
            this.Options = RegexOptions.None;
        }

        public Regular(string expression, RegexOptions options)
        {
            this.Expression = expression;
            this.Options = options;
        }

        public string Expression { protected set; get; }

        public RegexOptions Options { set; get; }
    }

    public class CrawlerOption
    {
        public CrawlerOption()
        {
            IsListUriTransform = false;
            IsContentUriTransform = false;

            IsTransContentImgUrl = true;

            IsCrawlerByNum = false;
            MaxCrawlerDays = 1;
            CrawlerDays = 1;

            MaxCrawlerNum = 1;
            CrawlerNum = 1;

            ListParamMethod = ParamTransMethod.Get;
            ContentParamMethod = ParamTransMethod.Get;
        }

        public bool IsListUriTransform { set; get; }

        public bool IsContentUriTransform { set; get; }

        public bool IsCrawlerByNum { set; get; }

        public bool IsTransContentImgUrl { set; get; }

        public int MaxCrawlerDays { set; get; }

        public int CrawlerDays { set; get; }

        public int MaxCrawlerNum { set; get; }

        public int CrawlerNum { set; get; }

        public ParamTransMethod ListParamMethod { set; get; }

        public ParamTransMethod ContentParamMethod { set; get; }
    }

    public enum ParamTransMethod
    {
        Get,

        Post,

        File,

        Dymanic
    }

    public enum CrawlerResultSign
    {
        Failed = 2,
        Success = 1,
        Repeat = 0
    }
}