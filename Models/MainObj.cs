using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace WpfBu.Models
{
    public class MainObj
    {
        public static string ConnectionString { get; set; } 
        public static string Account { get; set; }
        public static DBUtil Dbutil { get; set; }
        public static bool IsPostgres { get; set; }
    }

    public class DBUtil
    {
        public DataTable Runsql(string sql)
        {
            DataTable data = new DataTable();
            if (MainObj.IsPostgres)
            {
                var da = new NpgsqlDataAdapter(sql, MainObj.ConnectionString);
                da.Fill(data);
            }
            else
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, MainObj.ConnectionString);
                da.Fill(data);
            }
            return data;
        }

        public DataTable Runsql(string sql, Dictionary<string, object> par)
        {

            DataTable data = new DataTable();
            if (MainObj.IsPostgres)
            {
                var da = new NpgsqlDataAdapter(sql, MainObj.ConnectionString);
                foreach (string s in par.Keys)
                    da.SelectCommand.Parameters.AddWithValue(s, par[s]);
                da.Fill(data);
            }
            else
            {
                var da = new SqlDataAdapter(sql, MainObj.ConnectionString);
                foreach (string s in par.Keys)
                    da.SelectCommand.Parameters.AddWithValue(s, par[s]);
                da.Fill(data);
            }
            return data;
        }
    }
}
