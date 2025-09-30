using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Text.Json;
using assenteller;
using System.IO;
using System.Linq;

// wisselstand sensor 1 is links en 0 rechts 
class Program
{
    static async Task Main()
    {
        string connectionString = @"Server=NLUTRCSQL02-T\SQL01;Database=SSY_Strukton_IoT_Test;Trusted_Connection=True;";

        // Variabelen om de assen te berekenen 
        int deviceID = 0;
        int counter901_902 = 0;
        int counter901_903 = 0;
        int counter902_904 = 0;
        int counter902_910 = 12;
        int counter903_905 = 0;
        int counter903_906 = 0;
        int counter904_907 = 0;
        int counter904_E = 0;
        int counter905_910 = 4;
        int counter905_C = 11;
        int counter906_B = 0;
        int counter906_908 = 0;
        int counter907_909 = 0;
        int counter907_F = 16;
        int counter908_A = 0;
        int counter908_X = 0;
        int counter909_H = 8;
        int counter909_G = 0;
        int counter910_D = 0;
        int axles901 = 0;
        int axles902 = 0;
        int axles903 = 0;
        int axles904 = 0;
        int axles905 = 0;
        int axles906 = 0;
        int axles907 = 0;
        int axles908 = 0;
        int axles909 = 0;
        int axles910 = 0;
        char richting;

        // Sensoren koppelen aan variabelen
        const string wisselsensor1 = "7CC6C40700000873";
        const string wisselsensor2 = "7CC6C40700000894";
        const string wisselsensor3 = "7CC6C40700000851";
        const string wisselsensor4 = "7CC6C40700000876";
        const string wisselsensor5 = "7CC6C40700000890";
        const string wisselsensor6 = "7CC6C40700000643";
        const string wisselsensor7 = "7CC6C40700000891";
        const string wisselsensor8 = "7CC6C40700000636";
        const string wisselsensor9 = "7CC6C40700000880";
        const string wisselsensor10 = "7CC6C40700000606";
        const string wisselsensor11 = "7CC6C40700000608";
        List<string> last_IO_wisselsensor1 = new List<string>();
        List<string> last_IO_wisselsensor2 = new List<string>();
        List<string> last_IO_wisselsensor3 = new List<string>();
        List<string> last_IO_wisselsensor4 = new List<string>();
        List<string> last_IO_wisselsensor5 = new List<string>();
        List<string> last_IO_wisselsensor6 = new List<string>();
        List<string> last_IO_wisselsensor7 = new List<string>();
        List<string> last_IO_wisselsensor8 = new List<string>();
        List<string> last_IO_wisselsensor9 = new List<string>();
        List<string> last_IO_wisselsensor10 = new List<string>();
        List<string> last_IO_wisselsensor11 = new List<string>();

        // Van website data ophalen ,wisselstand sensorern
        using var client = new HttpClient();
        String url = "\r\nhttps://sam.strukton.com/web/TraceMe/Messages_Read?count=500";
        var response = await client.GetStringAsync(url);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);

        foreach (var message in apiResponse.Data)
        {
            switch (message.DevEUI)
            {
                case wisselsensor1:
                    last_IO_wisselsensor1.Add(message.IO);
                    break;
                case wisselsensor2:
                    last_IO_wisselsensor2.Add(message.IO);
                    break;
                case wisselsensor3:
                    last_IO_wisselsensor3.Add(message.IO);
                    break;
                case wisselsensor4:
                    last_IO_wisselsensor4.Add(message.IO);
                    break;
                case wisselsensor5:
                    last_IO_wisselsensor5.Add(message.IO);
                    break;
                case wisselsensor6:
                    last_IO_wisselsensor6.Add(message.IO);
                    break;
                case wisselsensor7:
                    last_IO_wisselsensor7.Add(message.IO);
                    break;
                case wisselsensor8:
                    last_IO_wisselsensor8.Add(message.IO);
                    break;
                case wisselsensor9:
                    last_IO_wisselsensor9.Add(message.IO);
                    break;
                case wisselsensor10:
                    last_IO_wisselsensor10.Add(message.IO);
                    break;
                case wisselsensor11:
                    last_IO_wisselsensor11.Add(message.IO);
                    break;
            }
        }
        int richting_wisselstand1 = last_IO_wisselsensor1[0][5];
        int richting_wisselstand2 = last_IO_wisselsensor2[0][5];
        int richting_wisselstand3 = last_IO_wisselsensor3[0][5];
        int richting_wisselstand4 = last_IO_wisselsensor4[0][5];
        int richting_wisselstand5 = last_IO_wisselsensor5[0][5];
        int richting_wisselstand6 = last_IO_wisselsensor6[0][5];
        int richting_wisselstand7 = last_IO_wisselsensor7[0][5];
        int richting_wisselstand8 = last_IO_wisselsensor8[0][5];
        int richting_wisselstand9 = last_IO_wisselsensor9[0][5];
        Console.WriteLine($"sensor 1: {last_IO_wisselsensor1[0]} en sensor 2:{last_IO_wisselsensor2[0]}");


        // Van database data ophalen ,WPS sensoren
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                string query = "SELECT * FROM dbo.WheelPassingMeasurements\r\n;";
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    deviceID = Convert.ToInt32(reader["DeviceID"]);
                    richting = Convert.ToChar(reader["Direction"]);

                    if (deviceID == 66)
                    {
                        axles901 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            //901 en 902 INGAAN
                            if (richting_wisselstand1 == 0)
                            {
                                counter901_902 = counter901_902 + axles901;
                            }
                            //901 en 903 INGAAN
                            else
                            {
                                counter901_903 = counter901_903 + axles901;
                            }
                        }
                        else
                        {
                            //901 en 902 UITGAAN
                            if (richting_wisselstand1 == 0)
                            {
                                counter901_902 = counter901_902 - axles901;
                            }
                            //901 en 903 UITGAAN
                            else
                            {
                                counter901_903 = counter901_903 - axles901;
                            }
                        }

                    }

                    else if (deviceID == 75)
                    {
                        axles902 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            //902 en 904 INGAAN
                            if (richting_wisselstand2 == 0)
                            {
                                counter901_902 = counter901_902 - axles902;
                                counter902_904 = counter902_904 + axles902;
                            }
                            //902 en 910 INGAAN
                            else
                            {
                                counter901_902 = counter901_902 - axles902;
                                counter902_910 = counter902_910 + axles902;
                            }
                        }
                        else
                        {
                            //902 en 904 UITGAAN
                            if (richting_wisselstand2 == 0)
                            {
                                counter902_904 = counter902_904 - axles902;
                                counter901_902 = counter901_902 + axles902;
                            }
                            //902 en 910 UITGAAN
                            else
                            {
                                counter902_910 = counter902_910 - axles902;
                                counter901_902 = counter901_902 + axles902;
                            }
                        }
                    }
                    else if (deviceID == 69)
                    {
                        axles904 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            //904 en 907 INGAAN
                            counter902_904 = counter902_904 - axles904;
                            if (richting_wisselstand4 == 0)
                            {
                                counter904_907 = counter904_907 + axles904;
                            }
                            //904 en E INGAAN
                            else
                            {
                                counter904_E = counter904_E + axles904;
                            }
                        }
                        else
                        {
                            counter902_904 = counter902_904 + axles904;
                            //904 en 907 UITGAAN
                            if (richting_wisselstand4 == 0)
                            {
                                counter904_907 = counter904_907 - axles904;
                            }
                            //904 en E UITGAAN
                            else
                            {
                                counter904_E = counter904_E - axles904;
                            }
                        }
                    }
                    else if (deviceID == 63)
                    {
                        axles907 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            counter904_907 = counter904_907 - axles907;

                            //907 en 909 INGAAN
                            if (richting_wisselstand7 == 0)
                            {
                                counter907_909 = counter907_909 + axles907;
                            }
                            //907 en F INGAAN
                            else
                            {
                                counter907_F = counter907_F + axles907;
                            }
                        }
                        else
                        {
                            counter904_907 = counter904_907 + axles907;
                            //907 en 909 UITGAAN
                            if (richting_wisselstand7 == 0)
                            {
                                counter907_909 = counter907_909 - axles907;
                            }
                            //907 en F UITGAAN
                            else
                            {
                                counter907_F = counter907_F - axles907;
                            }
                        }
                    }
                    else if (deviceID == 73)
                    {
                        axles909 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            counter907_909 = counter907_909 - axles907;

                            //909 en H INGAAN
                            if (richting_wisselstand9 == 0)
                            {
                                counter909_H = counter909_H + axles909;
                            }
                            //909 en G INGAAN
                            else
                            {
                                counter909_G = counter909_G + axles909;
                            }
                        }
                        else
                        {
                            counter907_909 = counter907_909 + axles907;
                            if (richting_wisselstand9 == 0)
                            {
                                counter909_H = counter909_H - axles909;
                            }
                            else
                            {
                                counter909_G = counter909_G - axles909;
                            }
                        }

                    }
                    else if (deviceID == 74)
                    {
                        axles903 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            counter901_903 = counter901_903 - axles903;
                            if (richting_wisselstand3 == 0)
                            {
                                counter903_905 = counter903_905 + axles903;
                            }
                            else
                            {
                                counter903_906 = counter903_906 + axles903;
                            }
                        }
                        else
                        {
                            counter901_903 = counter901_903 + axles903;
                            if (richting == 0)
                            {
                                counter903_905 = counter903_905 - axles903;
                            }
                            else
                            {
                                counter903_906 = counter903_906 - axles903;
                            }
                        }
                    }
                    else if (deviceID == 72)
                    {
                        axles905 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            counter903_905 = counter903_905 - axles905;
                            if (richting_wisselstand5 == 0)
                            {
                                counter905_910 = counter905_910 + axles905;
                            }
                            else
                            {
                                counter905_C = counter905_C + axles905;
                            }
                        }
                        else
                        {
                            counter903_905 = counter903_905 + axles905;
                            if (richting_wisselstand5 == 0)
                            {
                                counter905_910 = counter905_910 - axles905;
                            }
                            else
                            {
                                counter905_C = counter905_C - axles905;
                            }
                        }
                    }
                    else if (deviceID == 67 || deviceID == 70)
                    {
                        axles910 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L') {
                            if (deviceID == 67)
                            {
                                counter905_910 = counter905_910 - axles910;
                            }
                            else
                            {
                                counter902_910 = counter902_910 - axles910;
                            }
                            counter910_D = counter910_D + axles910;
                        }
                        else
                        {
                            counter910_D=counter910_D - axles910;
                            if(deviceID == 67)
                            {
                                counter905_910 = counter905_910 + axles910;
                            }
                            else
                            {
                                counter902_910 = counter902_910 + axles910;
                            }
                        }
                    }
                    else if (deviceID == 68)
                    {
                        axles906 = Convert.ToInt32(reader["TotalAxles"]);
                        if(richting == 'L')
                        {
                            counter903_906 = counter903_906 - axles906;
                            if(richting_wisselstand6== 0)
                            {
                                counter906_B = counter906_B + axles906; 
                            }
                            else
                            {
                                counter906_908 = counter906_908 + axles906;
                            }
                        }
                        else
                        {
                            counter903_906=counter903_906 + axles906;
                            if(richting_wisselstand6 == 0)
                            {
                                counter906_B=counter906_B - axles906;
                            }
                            else
                            {
                                counter906_908=counter906_B - axles906;
                            }
                        }
                    }
                    else if (deviceID == 71)
                    {
                        axles908 = Convert.ToInt32(reader["TotalAxles"]);
                        if (richting == 'L')
                        {
                            counter906_908= counter906_908 - axles908;
                            if(richting_wisselstand8== 0)
                            {
                                counter908_A = counter908_A + axles908;
                            }
                            else
                            {
                                counter908_X = counter908_X + axles908;
                            }
                        }
                        else
                        {
                            counter906_908 = counter906_908 + axles908;
                            if(richting_wisselstand8 == 0)
                            {
                                counter908_A= counter908_A - axles908;
                            }
                            else
                            {
                                counter908_X= counter908_X - axles908;
                            }
                        }
                    }

                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        Console.WriteLine("totale assen door 901-902: " + counter901_902);
        Console.WriteLine("totale assen door 902-904: " + counter902_904);
        Console.WriteLine("totale assen door 904-907: " + counter904_907);
        Console.WriteLine("totale assen door 904-E: " + counter904_E);
        Console.WriteLine("totale assen door 907-909: " + counter907_909);
        Console.WriteLine("totale assen door 907-F: " + counter907_F);
        Console.WriteLine("totale assen door 902-910: " + counter902_910);
        Console.WriteLine("totale assen door 901-903: " + counter901_903);
        Console.WriteLine("totale assen door 903-905: " + counter903_905);
        Console.WriteLine("totale assen door 903-906: " + counter903_906);
        Console.WriteLine("totale assen door 905-910: " + counter905_910);
        Console.WriteLine("totale assen door 910-D: " + counter910_D);
        Console.WriteLine("totale assen door 905-C: " + counter905_C);
        Console.WriteLine("totale assen door 906-B: " + counter906_B);
        Console.WriteLine("totale assen door 906-908: " + counter906_908);
        Console.WriteLine("totale assen door 908-A: " + counter908_A);
        Console.WriteLine("totale assen door 908-X: " + counter908_X);


    }
}
