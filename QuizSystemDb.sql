USE [master]
GO
/****** Object:  Database [StudentPortalDb]    Script Date: 13-May-25 7:52:21 PM ******/
CREATE DATABASE [StudentPortalDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'StudentPortalDb', FILENAME = N'C:\ProgramData\SOLIDWORKS Electrical\MSSQL12.TEW_SQLEXPRESS\MSSQL\DATA\StudentPortalDb.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'StudentPortalDb_log', FILENAME = N'C:\ProgramData\SOLIDWORKS Electrical\MSSQL12.TEW_SQLEXPRESS\MSSQL\DATA\StudentPortalDb_log.ldf' , SIZE = 816KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [StudentPortalDb] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [StudentPortalDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [StudentPortalDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [StudentPortalDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [StudentPortalDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [StudentPortalDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [StudentPortalDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [StudentPortalDb] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [StudentPortalDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [StudentPortalDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [StudentPortalDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [StudentPortalDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [StudentPortalDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [StudentPortalDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [StudentPortalDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [StudentPortalDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [StudentPortalDb] SET  ENABLE_BROKER 
GO
ALTER DATABASE [StudentPortalDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [StudentPortalDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [StudentPortalDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [StudentPortalDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [StudentPortalDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [StudentPortalDb] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [StudentPortalDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [StudentPortalDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [StudentPortalDb] SET  MULTI_USER 
GO
ALTER DATABASE [StudentPortalDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [StudentPortalDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [StudentPortalDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [StudentPortalDb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [StudentPortalDb] SET DELAYED_DURABILITY = DISABLED 
GO
USE [StudentPortalDb]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Questions]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Questions](
	[QuestionId] [int] IDENTITY(1,1) NOT NULL,
	[QuizId] [int] NULL,
	[QuestionText] [nvarchar](max) NULL,
	[OptionA] [nvarchar](max) NULL,
	[OptionB] [nvarchar](max) NULL,
	[OptionC] [nvarchar](max) NULL,
	[OptionD] [nvarchar](max) NULL,
	[CorrectOption] [nvarchar](50) NULL,
	[QuestionType] [nvarchar](50) NULL,
	[MinAcceptableValue] [float] NULL,
	[MaxAcceptableValue] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuizResults]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuizResults](
	[ResultId] [int] IDENTITY(1,1) NOT NULL,
	[QuizId] [int] NULL,
	[UserId] [int] NULL,
	[TakenDate] [datetime] NULL,
	[TotalScore] [float] NULL,
	[PercentageScore] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Quizzes]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Quizzes](
	[QuizId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[TotalQuestions] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[QuizId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Phone] [nvarchar](max) NOT NULL,
	[subscribed] [bit] NOT NULL,
 CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuspiciousLogs]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuspiciousLogs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[QuizId] [int] NULL,
	[UserId] [nvarchar](100) NULL,
	[EventType] [nvarchar](100) NULL,
	[QuestionId] [int] NULL,
	[TimeTaken] [float] NULL,
	[Timestamp] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsersData]    Script Date: 13-May-25 7:52:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Questions] ADD  DEFAULT ('MCQ') FOR [QuestionType]
GO
ALTER TABLE [dbo].[QuizResults] ADD  DEFAULT (getdate()) FOR [TakenDate]
GO
ALTER TABLE [dbo].[Quizzes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Quizzes] ADD  DEFAULT ((10)) FOR [TotalQuestions]
GO
ALTER TABLE [dbo].[UsersData] ADD  DEFAULT ('Student') FOR [Role]
GO
ALTER TABLE [dbo].[Questions]  WITH CHECK ADD FOREIGN KEY([QuizId])
REFERENCES [dbo].[Quizzes] ([QuizId])
GO
ALTER TABLE [dbo].[QuizResults]  WITH CHECK ADD FOREIGN KEY([QuizId])
REFERENCES [dbo].[Quizzes] ([QuizId])
GO
USE [master]
GO
ALTER DATABASE [StudentPortalDb] SET  READ_WRITE 
GO
