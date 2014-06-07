using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using AllCheckin.Contract;
using System.Collections.Generic;

namespace AllCheckin.DB
{
    public class AllCheckinSqlStorageProvider : IAllCheckinStorageProvider
    {
        private SqlConnection conn;

        public AllCheckinSqlStorageProvider()
        {
            this.conn = new SqlConnection();
            conn.ConnectionString = "Data Source=CFVBAIBAI-S01;Initial Catalog=kaifang;Integrated Security=SSPI;";
            conn.Open();
        }

        public IList<string> GetGivenNames()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM tbl_GivenName WHERE LEN(nvc_given_name) = 1";
                cmd.CommandType = CommandType.Text;

                var givenNames = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        givenNames.Add(reader.GetString(0));
                    }
                }

                return givenNames;
            }
        }

        public IList<SurName> GetSurNames()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM tbl_SurName";
                cmd.CommandType = CommandType.Text;

                var surNames = new List<SurName>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        surNames.Add(new SurName
                        {
                            Id = reader.GetString(0),
                            Chinese = reader.GetString(1),
                            Weight = reader.GetInt32(2),
                        });
                    }
                }

                return surNames;
            }
        }

        public IList<string> GetNames()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT nvc_name FROM tbl_Name";
                cmd.CommandType = CommandType.Text;

                var names = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add(reader.GetString(0));
                    }
                }

                return names;
            }
        }

        public IList<string> GetNamesOrderByCount()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT nvc_name FROM tbl_Name ORDER BY i_count DESC";
                cmd.CommandType = CommandType.Text;

                var names = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add(reader.GetString(0));
                    }
                }

                return names;
            }
        }

        public void SaveEntry(CheckinEntry entry)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_kf_NewCheckinEntry_1";
                cmd.CommandType = CommandType.StoredProcedure;

                AddParameter(cmd, "@nvc_name", entry.Name);
                AddParameter(cmd, "@vc_idcard_number", entry.Id);
                AddParameter(cmd, "@si_gendre", (short)entry.Gendre);
                AddParameter(cmd, "@dt2_birthdate", entry.Birthdate);
                AddParameter(cmd, "@nvc_address", entry.Address);
                AddParameter(cmd, "@vc_cellphone_number", entry.CellPhoneNumber);
                AddParameter(cmd, "@vc_telephone_number", entry.TelephoneNumber);
                AddParameter(cmd, "@vc_mailbox", entry.Mailbox);
                AddParameter(cmd, "@dt2_checkin_time", entry.CheckinTime);
                cmd.ExecuteNonQuery();
            }
        }

        public bool IsNameQueried(string name)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_kf_NameQueried_1";
                cmd.CommandType = CommandType.StoredProcedure;

                AddParameter(cmd, "@nvc_name", name);
                SqlParameter nameQueried = AddOutputParameter(cmd, "@b_name_queried", SqlDbType.Bit);

                cmd.ExecuteNonQuery();
                return (bool)nameQueried.Value;
            }
        }

        private static SqlParameter AddParameter<T>(SqlCommand cmd, string name, T value)
        {
            if (value == null)
            {
                return cmd.Parameters.AddWithValue(name, DBNull.Value);
            }
            else
            {
                return cmd.Parameters.AddWithValue(name, value);
            }
        }

        private static SqlParameter AddOutputParameter(SqlCommand cmd, string name, SqlDbType type)
        {
            return cmd.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = name,
                SqlDbType = type,
            });
        }

        public void Dispose()
        {
            this.conn.Dispose();
        }
    }
}