USE kaifang;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE dbo.sp_kf_NameQueried_1
    @nvc_name AS NVARCHAR(64),
    @b_name_queried AS BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM tbl_CheckinEntry
        WHERE nvc_name = @nvc_name
        AND LEN(vc_idcard_number) = 18)
    BEGIN
        SET @b_name_queried = 1
    END
    ELSE
    BEGIN
        SET @b_name_queried = 0
    END
END
GO
