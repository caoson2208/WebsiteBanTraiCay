using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebsiteBanTraiCay05.Models.Common
{
    public class StatisticalAccess
    {
        public static string strConnect = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public static StatisticalViewModel Statistical()
        {
            try
            {
                using (var connect = new SqlConnection(strConnect))
                {
                    connect.Open();
                    var item = connect.QueryFirstOrDefault<StatisticalViewModel>("sp_Statistical", commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thực thi thủ tục lưu trữ: " + ex.Message);
                return null; 
            }
        }
    }
}