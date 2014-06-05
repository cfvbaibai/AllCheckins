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
        CONSTRAINT [PK_CheckinEntry] PRIMARY KEY CLUSTERED 
        (
            [guid_id] ASC
        )
    ) ON [PRIMARY]

    CREATE NONCLUSTERED INDEX IX_CheckinEntry_Main ON [tbl_CheckinEntry] (vc_idcard_number ASC, nvc_name ASC)
    CREATE NONCLUSTERED INDEX IX_CheckinEntry_Name ON [tbl_CheckinEntry] (nvc_name ASC)
END
GO

SET ANSI_PADDING OFF
GO

