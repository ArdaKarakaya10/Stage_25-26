using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assenteller
{
    public class ExcelHandler
    {
        public void Export(List<data> sensors, List<data_wps> wpsData, List<data_wps> AxlesData)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");
            int row = 1;
            int row1 = 1;
            int row2 = 1;
            int spoor901_902 = 0;
            int spoor901_903 = 0;
            int axles901 = 0;
            //string richting_spoor_tijd = "N";


            List<data> tijd_ws1 = new List<data>();
            List<data> tijd_ws1_unique = new List<data>();

            List<data_wps> tijd_WPS = new List<data_wps>();
            List<data_wps> tijd_WPS_unique = new List<data_wps>();
            // Headers
            worksheet.Cell(row, 1).Value = "Tijd";
            worksheet.Cell(row, 2).Value = "WS1";
            worksheet.Cell(row, 3).Value = "WS2";
            worksheet.Cell(row, 4).Value = "WS3";
            worksheet.Cell(row, 5).Value = "WS4";
            worksheet.Cell(row, 6).Value = "WS5";
            worksheet.Cell(row, 7).Value = "WS6";
            worksheet.Cell(row, 8).Value = "WS7";
            worksheet.Cell(row, 9).Value = "WS8";
            worksheet.Cell(row, 10).Value = "WS9";
            worksheet.Cell(row, 11).Value = "Tijd WPS";
            worksheet.Cell(row, 12).Value = "WPS901";
            worksheet.Cell(row, 13).Value = "WPS902";
            worksheet.Cell(row, 14).Value = "WPS903";
            worksheet.Cell(row, 15).Value = "WPS904";
            worksheet.Cell(row, 16).Value = "WPS905";
            worksheet.Cell(row, 17).Value = "WPS906";
            worksheet.Cell(row, 18).Value = "WPS907";
            worksheet.Cell(row, 19).Value = "WPS908";
            worksheet.Cell(row, 20).Value = "WPS909";
            worksheet.Cell(row, 21).Value = "WPS910Z";
            worksheet.Cell(row, 24).Value = "Tijd";
            worksheet.Cell(row, 25).Value = "901-902";
            worksheet.Cell(row, 26).Value = "902-904";


            // Sensor data schrijven
            foreach (var sensor in sensors)
            {
                string richting = "R";
                if (sensor.Type == 4 && sensor.IO != "-")
                {
                    switch (sensor.DevEUI)
                    {
                        case "7CC6C40700000873":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 2).Value = richting;
                            tijd_ws1.Add(sensor);
                            break;
                        case "7CC6C40700000894":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 3).Value = richting;
                            break;
                        case "7CC6C40700000851":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 4).Value = richting;
                            break;
                        case "7CC6C40700000876":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 5).Value = richting;
                            break;
                        case "7CC6C40700000890":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 6).Value = richting;
                            break;
                        case "7CC6C40700000643":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 7).Value = richting;
                            break;
                        case "7CC6C40700000891":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 8).Value = richting;
                            break;
                        case "7CC6C40700000636":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 9).Value = richting;
                            break;
                        case "7CC6C40700000880":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 10).Value = richting;
                            break;
                        case "7CC6C40700000606":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 11).Value = richting;
                            break;
                        case "7CC6C40700000608":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 12).Value = richting;
                            break;
                    }
                }
            }

            // WPS data schrijven
            foreach (var msg in wpsData)
            {
                switch (msg.DeviceID)
                {
                    case 66:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 12).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 75:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 13).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 74:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 14).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 69:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 15).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 72:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 16).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 68:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 17).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 63:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 18).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 71:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 19).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 73:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 20).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 67:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 21).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case 70:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 22).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                }

            }




            tijd_WPS_unique = AxlesData
                .DistinctBy(x => x.Timestamp)
                .ToList();


            foreach (var s in tijd_WPS_unique) { 
            
            Console.WriteLine(s.DeviceID);
            }

            foreach (var wps_message in tijd_WPS_unique)
            {

                string richting = "N";
                if (wps_message.DeviceID == 66)
                {
                    for (int i = 0; i < tijd_ws1.Count - 1; i++)
                {
                    var tijd = tijd_ws1[i];
                    var tijd_next = tijd_ws1[i + 1];

                    if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                    {
                            richting = tijd.IO;
                            axles901 = wps_message.TotalAxles;

                            if (wps_message.Direction == "L")
                            {
                                if (richting[4] == '0')
                                {
                                    spoor901_902 += axles901;
                                }
                                else
                                {
                                    spoor901_903 += axles901;
                                }
                            }
                            else
                            {
                                if (richting[4] == '0')
                                {
                                    spoor901_902 -= axles901;
                                    break;

                                }
                                else
                                {
                                    spoor901_903 -= axles901;
                                    break;

                                }
                            }
                            break; 
                    }
                    if (DateTime.Parse(wps_message.Timestamp) < DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd_next.Received))
                    {
                        richting = tijd_next.IO;
                        //break;
                    }

                }
                    

                    row2++;
                    worksheet.Cell(row2, 22).Value = wps_message.Timestamp;
                    worksheet.Cell(2, 25).Value = "test";
                }
            }
            workbook.SaveAs(@"C:\Users\akaraka1\source\repos\assenteller\event.xlsx");

        }
    }
}
