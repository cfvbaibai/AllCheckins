USE [kaifang]
GO

/****** Object:  Table [dbo].[tbl_CheckinEntry]    Script Date: 2014/6/1 22:56:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tbl_CheckinEntry')
BEGIN
    CREATE TABLE [dbo].[tbl_CheckinEntry] (
        [guid_id] [uniqueidentifier] NOT NULL DEFAULT NEWSEQUENTIALID(),
        [nvc_name] [nvarchar](64) NULL,
        [vc_idcard_number] [varchar](32) NULL,
        [si_gendre] [smallint] NULL,
        [dt2_birthdate] [datetime2](7) NULL,
        [nvc_address] [nvarchar](256) NULL,
        [vc_cellphone_number] [varchar](32) NULL,
        [vc_telephone_number] [varchar](32) NULL,
        [vc_mailbox] [varchar](128) NULL,
        [dt2_checkin_time] [datetime2](7) NULL,
    CONSTRAINT [PK_tbl_CheckinEntry] PRIMARY KEY CLUSTERED 
    (
        [guid_id] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

