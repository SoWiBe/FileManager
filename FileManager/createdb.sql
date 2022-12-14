USE [FilesDB]
GO
/****** Object:  Table [dbo].[Files]    Script Date: 09.09.2022 19:27:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Files](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Filename] [nvarchar](50) NULL,
	[DataVisited] [nvarchar](50) NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Files] ON 

INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (1, N'DumpStack.log', N'09.09.2022 18:37:47')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (2, N'hiberfil.sys', N'09.09.2022 18:38:12')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (3, N'pagefile.sys', N'09.09.2022 18:38:12')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (4, N'swapfile.sys', N'09.09.2022 18:38:13')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (5, N'DumpStack.log.tmp', N'09.09.2022 18:38:13')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (6, N'DumpStack.log', N'09.09.2022 18:38:14')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (7, N'config.ini', N'09.09.2022 18:38:14')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (8, N'bootTel.dat', N'09.09.2022 18:44:20')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (9, N'hiberfil.sys', N'09.09.2022 18:44:28')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (10, N'aow_drv.log', N'09.09.2022 18:45:50')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (11, N'pagefile.sys', N'09.09.2022 18:46:02')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (12, N'swapfile.sys', N'09.09.2022 18:46:16')
INSERT [dbo].[Files] ([Id], [Filename], [DataVisited]) VALUES (13, N'unins000.dat', N'09.09.2022 19:02:17')
SET IDENTITY_INSERT [dbo].[Files] OFF
GO
