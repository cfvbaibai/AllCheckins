USE kaifang;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE dbo.sp_kf_NewCheckinEntry_1
    @nvc_name AS NVARCHAR(64),
    @vc_idcard_number AS VARCHAR(32),
    @si_gendre AS SMALLINT,
    @dt2_birthdate AS DATETIME2,
    @nvc_address AS NVARCHAR(256),
    @vc_cellphone_number AS VARCHAR(32),
    @vc_telephone_number AS VARCHAR(32),
    @vc_mailbox AS VARCHAR(128),
    @dt2_checkin_time AS DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    MERGE tbl_CheckinEntry AS [Target]
    USING (SELECT
            @nvc_name, @vc_idcard_number, @si_gendre, @dt2_birthdate, @nvc_address,
            @vc_cellphone_number, @vc_telephone_number, @vc_mailbox, @dt2_checkin_time
        ) AS [Source] (
            nvc_name, vc_idcard_number, si_gendre, dt2_birthdate, nvc_address,
            vc_cellphone_number, vc_telephone_number, vc_mailbox, dt2_checkin_time
        ) ON ([Target].nvc_name = [Source].nvc_name AND [Target].vc_idcard_number = [Source].vc_idcard_number)
    WHEN MATCHED THEN
        UPDATE SET [Target].nvc_name = [Source].nvc_name
    WHEN NOT MATCHED THEN
        INSERT (nvc_name, vc_idcard_number, si_gendre, dt2_birthdate, nvc_address,
            vc_cellphone_number, vc_telephone_number, vc_mailbox, dt2_checkin_time)
        VALUES ([Source].nvc_name, [Source].vc_idcard_number, [Source].si_gendre, [Source].dt2_birthdate, [Source].nvc_address,
            [Source].vc_cellphone_number, [Source].vc_telephone_number, [Source].vc_mailbox, [Source].dt2_checkin_time)
    ;
END
GO
