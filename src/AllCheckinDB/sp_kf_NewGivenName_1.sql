USE kaifang;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE dbo.sp_kf_NewGivenName_1
    @nvc_given_name AS NVARCHAR(16)
AS
BEGIN
    SET NOCOUNT ON;

    MERGE tbl_GivenName AS [Target]
    USING (SELECT @nvc_given_name) AS [Source] (nvc_given_name)
    ON ([Target].nvc_given_name = [Source].nvc_given_name)
    WHEN NOT MATCHED THEN
        INSERT VALUES ([Source].nvc_given_name)
    ;
END
GO
