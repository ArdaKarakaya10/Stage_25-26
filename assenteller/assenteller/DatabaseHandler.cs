using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using assenteller;
using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace assenteller
{
    public class DatabaseHandler
    {
        private readonly string connectionString = @"Server=NLUTRCSQL02-T\SQL01;Database=SSY_Strukton_IoT_Test;Trusted_Connection=True;";
        public List<data_wps> GetWpsData()
        {
            var result = new List<data_wps>();
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM dbo.WheelPassingMeasurements WHERE Timestamp > '2025-10-08 13:01:10.0000000'";
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new data_wps
                    {
                        DeviceID = Convert.ToInt32(reader["DeviceID"]),
                        Timestamp = Convert.ToString(reader["Timestamp"]),
                        TotalAxles = Convert.ToInt32(reader["TotalAxles"]),
                        Direction = Convert.ToString(reader["Direction"]),
                    });
                }
                result.Reverse();
            }
            return result;
        }
    }
}
