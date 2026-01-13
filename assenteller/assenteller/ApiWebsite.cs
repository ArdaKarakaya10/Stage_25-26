using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
namespace assenteller
{
    public class ApiWebsite
    {
        private const string servername = @"Server=NLHGLSRAPPL13;Database=IotMessages;Trusted_Connection=True;";

        public List<data> GetSensorDataAsync()
        {
            var result = new List<data>();
            using (SqlConnection connection = new SqlConnection(servername))
            {
                string query = "SELECT * FROM [IotMessages].[dbo].[TraceMeMessages]WHERE Received>'2-12-2025  13:53:05'";
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new data
                    {
                        Received = Convert.ToString(reader["Received"]),
                        DevEUI = Convert.ToString(reader["DevEUI"]),
                        IO = Convert.ToString(Convert.ToInt32(reader["IO"]), 2).PadLeft(8, '0'),
                        Type = Convert.ToInt32(reader["Type"]),
                    });
                }

                result.Reverse();
            }
            return result;
        }
    }
}
