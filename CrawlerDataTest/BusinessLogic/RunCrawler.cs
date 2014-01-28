using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.Configuration;
using System.Web;
using CrawlerDataTest.DataAccess.Entities;
using CrawlerDataTest.DataAccess.Repository;
using ITS.Crawler;

namespace CrawlerDataTest.BusinessLogic
{
    /// <summary>
    /// 网络爬虫执行类
    /// </summary>
    public class RunCrawler
    {
        private CrawlerFactory factory;

        private BaseCrawler crawler;

        public RunCrawler()
        {
            factory = new CrawlerFactory();
        }

        /// <summary>
        /// 网络爬虫执行
        /// </summary>
        public void CrawlerStart(string url)
        {
            WriteTxt(string.Format("开始执行爬虫任务。时间：{0}", DateTime.Now.ToString()));
            GetInfos(url);
            WriteTxt(string.Format("爬虫任务执行完成。时间：{0}", DateTime.Now.ToString()));
        }

        /// <summary>
        /// 抓取数据
        /// </summary>
        private void GetInfos(string url)
        {
            WriteTxt(string.Format("正在执行爬虫任务,请等待。。。时间：{0}", DateTime.Now.ToString()));
            GetInfo(url);
        }

        private void GetInfo(string url)
        {
            crawler = factory.GetCrawlerHelper(url);

            string resultMessages = crawler.Start(url);
            WriteTxt(resultMessages);

        }

        /// <summary>
        /// 将内容信息写入文件中
        /// </summary>
        /// <param name="dataStr"></param>
        private void WriteTxt(string dataStr)
        {
            object obj = new object();
            System.IO.StreamWriter writer = null;
            try
            {
                lock (obj)
                {
                    // 写入文件
                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    string path = string.Empty;
                    string filename = DateTime.Now.Day.ToString() + ".txt";

                    //path = Directory.GetCurrentDirectory() + "/Logs/" + year + "/" + month;
                    try
                    {
                        path = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    catch
                    {
                        path = "D:";
                    }
                    path = path + "/CrawlerDataLog/" + year + "/" + month;
                    //如果目录不存在则创建
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    System.IO.FileInfo file = new System.IO.FileInfo(path + "/" + filename);

                    writer = new System.IO.StreamWriter(file.FullName, true);//文件不存在就创建,true表示追

                    writer.WriteLine(dataStr);
                    //writer.WriteLine("--------------------------------------------------------" + DateTime.Now.ToString() + "    第" + signNum.ToString() + "条数据");
                    writer.WriteLine();
                }
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
    }
}
