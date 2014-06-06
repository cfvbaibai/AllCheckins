USE kaifang
GO

CREATE TABLE tbl_SurName
(
    nvc_sur_name NVARCHAR(16) NOT NULL PRIMARY KEY,
    i_weight INT NOT NULL DEFAULT 0
)