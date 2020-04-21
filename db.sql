CREATE TABLE [dbo].[iis_log_files](
	[rowId] [int] IDENTITY(1,1) NOT NULL,
	[activeID] [bit] NOT NULL,
	[creationDate] [datetime] NOT NULL,
	[Iis_Id] [int] NULL,
	[Date] [datetime] NULL,
	[S_ip] [varchar](2048) NULL,
	[Cs_method] [varchar](2048) NULL,
	[Cs_uri_stem] [varchar](2048) NULL,
	[Cs_uri_query] [varchar](2048) NULL,
	[S_port] [varchar](2048) NULL,
	[Cs_username] [varchar](2048) NULL,
	[C_ip] [varchar](2048) NULL,
	[Cs_user_agent] [varchar](2048) NULL,
	[Sc_status] [varchar](2048) NULL,
	[Sc_substatus] [int] NULL,
	[Sc_win32_status] [int] NULL,
	[Time_taken] [int] NULL,
 CONSTRAINT [PK__iis_log___4B58DB807919578B] PRIMARY KEY CLUSTERED 
(
	[rowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[iis_log_files] ADD  CONSTRAINT [DF__iis_log_f__activ__182C9B23]  DEFAULT ((1)) FOR [activeID]
GO

ALTER TABLE [dbo].[iis_log_files] ADD  CONSTRAINT [DF__iis_log_f__creat__1920BF5C]  DEFAULT (getdate()) FOR [creationDate]
GO
