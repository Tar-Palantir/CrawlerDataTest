USE [master]
GO
/****** Object:  Database [CrawlerData]    Script Date: 05/09/2013 18:44:42 ******/
CREATE DATABASE [CrawlerData] ON  PRIMARY 
( NAME = N'CrawlerData', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\CrawlerData.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CrawlerData_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\CrawlerData_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [CrawlerData] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CrawlerData].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CrawlerData] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [CrawlerData] SET ANSI_NULLS OFF
GO
ALTER DATABASE [CrawlerData] SET ANSI_PADDING OFF
GO
ALTER DATABASE [CrawlerData] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [CrawlerData] SET ARITHABORT OFF
GO
ALTER DATABASE [CrawlerData] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [CrawlerData] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [CrawlerData] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [CrawlerData] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [CrawlerData] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [CrawlerData] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [CrawlerData] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [CrawlerData] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [CrawlerData] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [CrawlerData] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [CrawlerData] SET  DISABLE_BROKER
GO
ALTER DATABASE [CrawlerData] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [CrawlerData] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [CrawlerData] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [CrawlerData] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [CrawlerData] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [CrawlerData] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [CrawlerData] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [CrawlerData] SET  READ_WRITE
GO
ALTER DATABASE [CrawlerData] SET RECOVERY FULL
GO
ALTER DATABASE [CrawlerData] SET  MULTI_USER
GO
ALTER DATABASE [CrawlerData] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [CrawlerData] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'CrawlerData', N'ON'
GO
USE [CrawlerData]
GO
/****** Object:  Table [dbo].[ContentInfo]    Script Date: 05/09/2013 18:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ContentInfo](
	[ID] [char](36) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[ContentAbstract] [nvarchar](100) NULL,
	[InformationSource] [nvarchar](100) NULL,
	[PublishTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ContentInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContentInfo', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信息内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContentInfo', @level2type=N'COLUMN',@level2name=N'Content'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内容摘要' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContentInfo', @level2type=N'COLUMN',@level2name=N'ContentAbstract'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信息来源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContentInfo', @level2type=N'COLUMN',@level2name=N'InformationSource'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发布时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContentInfo', @level2type=N'COLUMN',@level2name=N'PublishTime'
GO
