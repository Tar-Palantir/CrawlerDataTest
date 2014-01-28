/* ***********************警告********************************************
 * 如果引用请不要删除本注释!本代码受版权法和国际条约保护,如未经授权擅自复制或散发本代码(或其中任何部份),
 * 将受到严厉的法律制裁,并将在法律许可的最大限度内受到起诉!
 * 版权所有(C)天府软件园有限公司  2009-2013
 * 公司:成都天府软件园有限公司 Chengdu Tianfu Software Park Co., Ltd.
 * 地址: 中国 成都 高新区天府大道天府软件园C8号楼东侧二楼 610041 
 * Address:	2F, East Tower, C8, Tianfu Software Park, Tianfu Avenue, Chengdu, P.R.China, 610041
 * E-Mail :  	chenming@tianfusoftwarepark.com
 * Created by:	陈明(Astaldo) at  2013/3/20 11:39:37
 * Coder by:	陈明(Astaldo)  at 2013/3/20 11:39:37
 * Modified by:	陈明(Astaldo)  at  2013/3/20 11:39:37
 * CLR版本： 	4.0.30319.296
 * 机器名称：   ITS-CHENMING
 * 文 件 名：   ManageSiteBaseModel
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;
using CrawlerDataTest.DataAccess.Entities;
using ITS.Crawler;

namespace CrawlerDataTest.DataAccess.Repository
{
    internal class ContentInfoRepository
    {
        protected DbContext context;

        internal ContentInfoRepository()
        {
            context = new CrawlerDataEntities();
        }

        protected virtual IQueryable<ContentInfo> QueryObject
        {
            get { return context.Set<ContentInfo>(); }
        }

        internal virtual void Create(ContentInfo entity, out ResultStatus status)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                }

                context.Set<ContentInfo>().Add(entity);
                context.SaveChanges();

                status = new ResultStatus() { ResultSign = CrawlerResultSign.Success, Message = "添加成功" };
            }
            catch
            {
                status = new ResultStatus() { ResultSign = CrawlerResultSign.Failed, Message = "添加错误" };
            }
        }

        internal virtual void Update(ContentInfo entity, out ResultStatus status)
        {
            try
            {
                if (context.Entry<ContentInfo>(entity).State != EntityState.Modified)
                {
                    var oldeEntity = this.GetByID(entity.ID);
                    var stateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(oldeEntity);
                    stateEntry.ApplyCurrentValues(entity);
                    stateEntry.SetModified();
                }
                context.SaveChanges();

                status = new ResultStatus() { ResultSign = CrawlerResultSign.Success, Message = "更新成功" };
            }
            catch
            {
                status = new ResultStatus() { ResultSign = CrawlerResultSign.Failed, Message = "更新错误" };
            }
        }

        internal virtual void Delete(string id, out ResultStatus status)
        {
            try
            {
                var entity = this.GetByID(id);
                context.Set<ContentInfo>().Remove(entity);
                context.SaveChanges();

                status = new ResultStatus() { ResultSign = CrawlerResultSign.Success, Message = "删除成功" };
            }
            catch
            {
                status = new ResultStatus() { ResultSign = CrawlerResultSign.Failed, Message = "删除错误" };
            }
        }

        internal virtual ContentInfo GetByID(string id)
        {
            return QueryObject.FirstOrDefault(p => p.ID == id);
        }

        internal virtual IList<ContentInfo> GetAll()
        {
            return QueryObject.ToList();
        }
    }
}