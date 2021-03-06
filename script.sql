USE [dictionary]
GO
/****** Object:  Table [dbo].[Entry]    Script Date: 27.02.2019 23:35:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Entry](
	[EntryId] [uniqueidentifier] NOT NULL,
	[Entry] [nvarchar](max) NULL,
	[Time] [datetime] NULL,
	[UserId] [uniqueidentifier] NULL,
	[VoteMinus] [int] NULL,
	[VotePlus] [int] NULL,
	[TitleId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[EntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Title]    Script Date: 27.02.2019 23:35:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Title](
	[TitleId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[UserId] [uniqueidentifier] NULL,
	[Time] [datetime] NULL,
	[Category] [nvarchar](20) NULL,
	[VoteMinus] [int] NULL,
	[VotePlus] [int] NULL,
	[TotalEntry] [int] NULL,
	[EntryId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[TitleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 27.02.2019 23:35:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[PasswordHash] [varbinary](max) NULL,
	[PasswordSalt] [varbinary](max) NULL,
	[Role] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voted]    Script Date: 27.02.2019 23:35:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voted](
	[VotedId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[EntryId] [uniqueidentifier] NULL,
	[IsVotedPlus] [bit] NULL,
	[IsVotedMinus] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[VotedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Entry] ADD  DEFAULT (newid()) FOR [EntryId]
GO
ALTER TABLE [dbo].[Entry] ADD  DEFAULT (getdate()) FOR [Time]
GO
ALTER TABLE [dbo].[Entry] ADD  DEFAULT ((0)) FOR [VoteMinus]
GO
ALTER TABLE [dbo].[Entry] ADD  DEFAULT ((0)) FOR [VotePlus]
GO
ALTER TABLE [dbo].[Title] ADD  DEFAULT (newid()) FOR [TitleId]
GO
ALTER TABLE [dbo].[Title] ADD  DEFAULT (getdate()) FOR [Time]
GO
ALTER TABLE [dbo].[Title] ADD  DEFAULT ((0)) FOR [VoteMinus]
GO
ALTER TABLE [dbo].[Title] ADD  DEFAULT ((0)) FOR [VotePlus]
GO
ALTER TABLE [dbo].[Title] ADD  DEFAULT ((0)) FOR [TotalEntry]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Voted] ADD  DEFAULT (newid()) FOR [VotedId]
GO
ALTER TABLE [dbo].[Entry]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Title]  WITH CHECK ADD FOREIGN KEY([EntryId])
REFERENCES [dbo].[Entry] ([EntryId])
GO
ALTER TABLE [dbo].[Title]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Voted]  WITH CHECK ADD FOREIGN KEY([EntryId])
REFERENCES [dbo].[Entry] ([EntryId])
GO
ALTER TABLE [dbo].[Voted]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
