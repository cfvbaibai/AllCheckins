USE kaifang
GO

CREATE TABLE tbl_SurName
(
    vc_sur_name_id VARCHAR(32) NOT NULL PRIMARY KEY,
    nvc_sur_name_ch NVARCHAR(16),
    i_weight INT NOT NULL DEFAULT 0
)