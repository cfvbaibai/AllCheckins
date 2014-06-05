USE kaifang
GO

CREATE TABLE tbl_Name
(
    guid_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    nc_first_char NCHAR(1) NOT NULL,
    nc_second_char NCHAR(1) NOT NULL,
    nvc_name NVARCHAR(32) NOT NULL,
    i_count INTEGER NOT NULL
)