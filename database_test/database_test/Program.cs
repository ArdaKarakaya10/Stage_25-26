using database_test;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;

class Program
{
    static void Main()
    {
        List<WagonStatus> statuses = new List<WagonStatus>();
        long Identification = 0;
        int DeviceID=0;
        DateTime Timestamp=DateTime.MinValue;
        char Richting='R';
        int Speed = 0;
        string Spoor = "NAN";
        string connectionString = @"Server=NLUTRCSQL02-T\SQL01;Database=SSY_Strukton_IoT_Test;Trusted_Connection=True;";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                string query = "SELECT Identification, DeviceID, Timestamp, Direction, Speed FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY Identification ORDER BY Timestamp DESC) AS rn FROM dbo.WheelPassingMeasurements) t WHERE rn=1;";
                connection.Open();
                SqlCommand cmd= new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Identification"]}, {reader["DeviceID"]}, {reader["Timestamp"]},{reader["Direction"]},{reader["Speed"]}");
                    Identification= Convert.ToInt64(reader["Identification"]);
                    DeviceID = Convert.ToInt32(reader["DeviceID"]);
                    Timestamp = Convert.ToDateTime(reader["Timestamp"]);
                    Richting = Convert.ToChar(reader["Direction"]);
                    Speed = Convert.ToInt32(reader["Speed"]);
                    if(DeviceID == 35 && Richting == 'L')
                    {
                        if (Speed >= 10)
                        {
                            Console.WriteLine("Gaat naar spoor C");
                            Spoor = "C";
                        }
                        else
                        {
                            Console.WriteLine("Gaat waarschijnlijk naar sensor 61");
                            Spoor = "Gaat na sensor 61";
                        }
                    }
                    else if (DeviceID == 35 && Richting == 'R')
                    {
                        Console.WriteLine("Gaat naar spoor D");
                        Spoor = "Gaat na sensor 65 of spoor D";
                    }

                    else if (DeviceID == 61 && Richting == 'L')
                    {
                        Console.WriteLine("Gaat naar spoor A");
                        Spoor = "A";
                    }
                    else if (DeviceID == 61 && Richting == 'R')
                    {
                        Console.WriteLine("Gaat naar spoor B");
                        Spoor = "B";
                    }
                    else if( DeviceID == 65 && Richting == 'L')
                    {
                        Console.WriteLine("Gaat naar spoor E");
                        Spoor = "E";
                    }
                    else
                    {
                        Console.WriteLine("Gaat naar spoor F");
                        Spoor = "F";
                    }

                    WagonStatus status = new WagonStatus
                    {
                        identification = Identification,
                        speed = Speed,
                        deviceID = DeviceID,
                        direction = Richting,
                        timestamp = Timestamp,
                        spoor=Spoor,
                    };
                    statuses.Add(status);

                    string json = JsonSerializer.Serialize(statuses, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine("\n JSON: "+ json);
                    string filePath = @"C:\Users\akaraka1\OneDrive - Strukton\Bureaublad\test\plattegrond\src\assets\status.json";
                    File.WriteAllText(filePath, json);
                   
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        Console.WriteLine("ID: " + Identification + " DEVICE ID : "+DeviceID + " timestamp: "+ Timestamp + " richtin: "+ Richting+ " speed: "+ Speed);
    }
}
