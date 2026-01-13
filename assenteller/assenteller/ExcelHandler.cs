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
       
        const int dev901 =78;
        const int dev902 =63;
        const int dev903 =70;
        const int dev904 =71;
        const int dev905 =69;
        const int dev906 =74;
        const int dev907 =75;
        const int dev908 =77;
        const int dev909 =73;
        const int dev910L =67;
        const int dev910R =66;
        public void Export(List<data> sensors, List<data_wps> wpsData, List<data_wps> AxlesData)
        {

            // Excel sheet maken
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            // Excel rijen
            int row = 1;
            int row1 = 1;
            int row2 = 1;


            // Spoor en axles 
            int spoor901_902 = 0;
            int spoor901_903 = 0;
            int axles901 = 0;

            int spoor902_904 = 0;
            int spoor902_910N = 6;
            int axles902 = 0;

            int spoor903_905 = 0;
            int spoor903_906 = 0;
            int axles903 = 0;

            int spoor904_907 = 0;
            int spoor904_E = 6;
            int axles904 = 0;

            int spoor905_910Z = 0;
            int spoor905_C = 9;
            int axles905 = 0;

            int spoor906_B = 12;
            int spoor906_908 = 0;
            int axles906 = 0;

            int spoor907_909 = 0;
            int spoor907_F = 8;
            int axles907 = 0;

            int spoor908_A = 8;
            int spoor908_X = 6;
            int axles908 = 0;

            int spoor909_H = 6;
            int spoor909_G = 0;
            int axles909 = 0;

            int spoor910_D = 8;
            int axles910_N = 0;
            int axles910_Z = 0;

            //tijden per wisselstand sensor
            List<data> tijd_ws1 = new List<data>();
            List<data> tijd_ws2 = new List<data>();
            List<data> tijd_ws3 = new List<data>();
            List<data> tijd_ws4 = new List<data>();
            List<data> tijd_ws5 = new List<data>();
            List<data> tijd_ws6 = new List<data>();
            List<data> tijd_ws7 = new List<data>();
            List<data> tijd_ws8 = new List<data>();
            List<data> tijd_ws9 = new List<data>();



            List<data_wps> tijd_WPS = new List<data_wps>();
            List<data_wps> tijd_WPS_unique = new List<data_wps>();
            List<data_wps> tijd_WPS_unique_1 = new List<data_wps>();

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
            worksheet.Cell(row, 22).Value = "WPS910N";
            worksheet.Cell(2, 24).Value = "Assen";
            worksheet.Cell(row, 25).Value = "901-902";
            worksheet.Cell(row, 26).Value = "902-904";
            worksheet.Cell(row, 27).Value = "902-910";
            worksheet.Cell(row, 28).Value = "904-907";
            worksheet.Cell(row, 29).Value = "904-E";
            worksheet.Cell(row, 30).Value = "907-909";
            worksheet.Cell(row, 31).Value = "907-F";
            worksheet.Cell(row, 32).Value = "909-H";
            worksheet.Cell(row, 33).Value = "901-903";
            worksheet.Cell(row, 34).Value = "903-905";
            worksheet.Cell(row, 35).Value = "903-906";
            worksheet.Cell(row, 36).Value = "905-910";
            worksheet.Cell(row, 37).Value = "905-C";
            worksheet.Cell(row, 38).Value = "906-B";
            worksheet.Cell(row, 39).Value = "906-908";
            worksheet.Cell(row, 40).Value = "908-A";
            worksheet.Cell(row, 41).Value = "908-X";
            worksheet.Cell(row, 42).Value = "910-D";
            worksheet.Cell(row, 43).Value = "909-G";


            // Wisselstand sensor deel in excel
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
                            tijd_ws2.Add(sensor);
                            break;
                        case "7CC6C40700000851":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 4).Value = richting;
                            tijd_ws3.Add(sensor);
                            break;
                        case "7CC6C40700000876":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 5).Value = richting;
                            tijd_ws4.Add(sensor);
                            break;
                        case "7CC6C40700000890":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 6).Value = richting;
                            tijd_ws5.Add(sensor);
                            break;
                        case "7CC6C40700000643":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 7).Value = richting;
                            tijd_ws6.Add(sensor);
                            break;
                        case "7CC6C40700000891":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 8).Value = richting;
                            tijd_ws7.Add(sensor);
                            break;
                        case "7CC6C40700000636":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 9).Value = richting;
                            tijd_ws8.Add(sensor);
                            break;
                        case "7CC6C40700000880":
                            row++;
                            if (sensor.IO[4] == '1')
                            {
                                richting = "L";
                            }
                            worksheet.Cell(row, 1).Value = sensor.Received;
                            worksheet.Cell(row, 10).Value = richting;
                            tijd_ws9.Add(sensor);
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


           
            // Alle dubbele eruit
            tijd_WPS_unique_1 = wpsData
                .DistinctBy(x => x.Timestamp)
                .ToList();

            // WPS sensor deel in excel

            foreach (var msg in tijd_WPS_unique_1)
            {
                switch (msg.DeviceID)
                {
                    case dev901:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 12).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev902:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 13).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev903:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 14).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev904:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 15).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev905:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 16).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev906:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 17).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev907:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 18).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev908:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 19).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev909:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 20).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev910L:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 21).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                    case dev910R:
                        row1++;
                        worksheet.Cell(row1, 11).Value = msg.Timestamp;
                        worksheet.Cell(row1, 22).Value = $"{msg.TotalAxles}{msg.Direction}";
                        tijd_WPS.Add(msg);
                        break;
                }

            }



            // Alle dubbele eruit
            tijd_WPS_unique = AxlesData
                .DistinctBy(x => x.Timestamp)
                .ToList();

            foreach (var wps_message in tijd_WPS_unique)
            {

                string richting = "N";

                // Juiste device vinden
                if (wps_message.DeviceID == dev901)
                {

                    for (int i = 0; i < tijd_ws1.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws1[i];
                        var tijd_next = tijd_ws1[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles901 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles901 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                            spoor901_902 += axles901;
                            }
                            else
                            {
                                spoor901_903 += axles901;
                                
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - L
                            if (richting[4] == '0')
                            {
                                spoor901_902 -= axles901;

                            }
                            else
                            {
                                spoor901_903 -= axles901;

                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev902)
                {

                    for (int i = 0; i < tijd_ws2.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws2[i];
                        var tijd_next = tijd_ws2[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles902 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles902 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor901_902 -= axles902;
                                spoor902_904 += axles902;
                            }
                            else
                            {
                                spoor901_902 -= axles902;
                                spoor902_910N += axles902;
                                
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor902_904 -= axles902;
                                spoor901_902 += axles902;
                            }
                            else
                            {
                                spoor902_910N -= axles902;
                                spoor901_902 += axles902;
                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev904)
                {
                    for (int i = 0; i < tijd_ws4.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws4[i];
                        var tijd_next = tijd_ws4[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles904 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles904 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor902_904 -= axles904;
                                spoor904_907 += axles904;
                            }
                            else
                            {
                                spoor902_904 -= axles904;
                                spoor904_E += axles904;
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor904_907 -= axles904;
                                spoor902_904 += axles904;
                            }
                            else
                            {
                                spoor904_E -= axles904;
                                spoor902_904 += axles904;
                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev907)
                {
                    for (int i = 0; i < tijd_ws7.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws7[i];
                        var tijd_next = tijd_ws7[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles907 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles907 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor904_907 -= axles907;
                                spoor907_909 += axles907;
                            }
                            else
                            {
                                spoor904_907 -= axles907;
                                spoor907_F += axles907;
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor907_909 -= axles907;
                                spoor904_907 += axles907;
                            }
                            else
                            {
                                spoor907_F -= axles907;
                                spoor904_907 += axles907;
                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev909)
                {
                    for (int i = 0; i < tijd_ws9.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws9[i];
                        var tijd_next = tijd_ws9[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles909 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles909 = wps_message.TotalAxles;
                            break;
                        }

                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor907_909 -= axles909;
                                spoor909_H += axles909;
                            }
                            else
                            {
                                spoor907_909 -= axles909;
                                spoor909_G += axles909;
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor909_H -= axles909;
                                spoor907_909 += axles909;
                            }
                            else
                            {
                                spoor909_G -= axles909;
                                spoor907_909 += axles909;
                            }
                        }
                    }
                

                else if (wps_message.DeviceID == dev903)
                {
                    //andere ook for apart doen want kan zo zijn dat zelfs bij else if de next nieuwe is bij tweede iteratie i+1 en dan stopt de programma bij een gehele for loop
                    for (int i = 0; i < tijd_ws3.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd_eerste = tijd_ws3[0];
                        var tijd = tijd_ws3[i];
                        var tijd_next = tijd_ws3[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd_eerste.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd_eerste.IO;
                            axles903 = wps_message.TotalAxles;
                            break;
                        }

                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            

                            richting = tijd_next.IO;
                            axles903 = wps_message.TotalAxles;
                            break;
                        }
                    }
                    // Richting trein bepalen gaat hij na binnen of buiten
                    if (wps_message.Direction == "L")
                    {
                        // Bepalen waar assen opgeteld moeten worden - R
                        if (richting[4] == '0')
                        {
                            spoor901_903 -= axles903;
                            spoor903_905 += axles903;
                        }
                        else
                        {
                            spoor901_903 -= axles903;
                            spoor903_906 += axles903;
                        }
                    }
                    // Richting trein bepalen gaat hij na binnen of buiten
                    else
                    {
                        // Bepalen waar assen opgeteld moeten worden - R
                        if (richting[4] == '0')
                        {
                            spoor903_905 -= axles903;
                            spoor901_903 += axles903;
                        }
                        else
                        {
                            spoor903_906 -= axles903;
                            spoor901_903 += axles903;
                        }
                    }
                }

                else if (wps_message.DeviceID == dev905)
                {
                    for (int i = 0; i < tijd_ws5.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws5[i];
                        var tijd_next = tijd_ws5[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles905 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles905 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor903_905 -= axles905;
                                spoor905_910Z += axles905;
                            }
                            else
                            {
                                spoor903_905 -= axles905;
                                spoor905_C += axles905;
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor905_910Z -= axles905;
                                spoor903_905 += axles905;

                            }
                            else
                            {
                                spoor905_C -= axles905;
                                spoor903_905 += axles905;

                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev906)
                {
                    for (int i = 0; i < tijd_ws6.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws6[i];
                        var tijd_next = tijd_ws6[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles906 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            
                            richting = tijd_next.IO;
                            axles906 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor903_906 -= axles906;
                                spoor906_B += axles906;
                            }
                            else
                            {
                                spoor903_906 -= axles906;
                                spoor906_908 += axles906;
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor906_B -= axles906;
                                spoor903_906 += axles906;
                            }
                            else
                            {
                                spoor906_908 -= axles906;
                                spoor903_906 += axles906;
                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev908)
                {
                    for (int i = 0; i < tijd_ws8.Count - 1; i++)
                    {
                        // Juiste tijd van wisselstand vinden
                        var tijd = tijd_ws8[i];
                        var tijd_next = tijd_ws8[i + 1];
                        var marge = TimeSpan.FromSeconds(2);

                        // Tijd bepalen als de laatste WPS sensor nieuwer is dan laatste wisselstand tijd
                        if (DateTime.Parse(wps_message.Timestamp) > DateTime.Parse(tijd.Received))
                        {
                            // richting en assen bepalen
                            richting = tijd.IO;
                            axles908 = wps_message.TotalAxles;
                            break;
                        }
                        // Tijd bepalen als wps tijd ergens tussen de wisselstand sensor waardes zit
                        else if (DateTime.Parse(wps_message.Timestamp) <= DateTime.Parse(tijd.Received) && DateTime.Parse(wps_message.Timestamp) >= DateTime.Parse(tijd_next.Received))
                        {
                            

                            richting = tijd_next.IO;
                            axles908 = wps_message.TotalAxles;
                            break;
                        }
                    }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        if (wps_message.Direction == "L")
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor906_908 -= axles908;
                                spoor908_A += axles908;
                            }
                            else
                            {
                                spoor906_908 -= axles908;
                                spoor908_X += axles908;
                            }
                        }
                        // Richting trein bepalen gaat hij na binnen of buiten
                        else
                        {
                            // Bepalen waar assen opgeteld moeten worden - R
                            if (richting[4] == '0')
                            {
                                spoor908_A -= axles908;
                                spoor906_908 += axles908;

                            }
                            else
                            {
                                spoor908_X -= axles908;
                                spoor906_908 += axles908;
                            }
                        }
                    }
                
                else if (wps_message.DeviceID == dev910R || wps_message.DeviceID == dev910L)
                {
                    if (wps_message.DeviceID == dev910R)
                    {
                        if (wps_message.Direction == "L")
                        {
                            axles910_N = wps_message.TotalAxles;
                            spoor902_910N -= axles910_N;
                            spoor910_D += axles910_N;
                        }
                        else
                        {
                            axles910_N = wps_message.TotalAxles;
                            spoor910_D -= axles910_N;
                            spoor902_910N += axles910_N;
                        }
                    }
                    else
                    {
                        if (wps_message.Direction == "L")
                        {
                            axles910_Z = wps_message.TotalAxles;
                            spoor905_910Z -= axles910_Z;
                            spoor910_D += axles910_Z;
                        }
                        else
                        {
                            axles910_Z = wps_message.TotalAxles;
                            spoor910_D -= axles910_Z;
                            spoor905_910Z += axles910_Z;
                        }
                    }
                }
                worksheet.Cell(2, 26).Value = spoor903_905;

            }
            // Na alle loops en berekeningen
                worksheet.Cell(2, 25).Value = spoor901_902;
                worksheet.Cell(2, 26).Value = spoor902_904;
                worksheet.Cell(2, 27).Value = spoor902_910N;
                worksheet.Cell(2, 28).Value = spoor904_907;
                worksheet.Cell(2, 29).Value = spoor904_E;
                worksheet.Cell(2, 30).Value = spoor907_909;
                worksheet.Cell(2, 31).Value = spoor907_F;
                worksheet.Cell(2, 32).Value = spoor909_H;
                worksheet.Cell(2, 33).Value = spoor901_903;
                worksheet.Cell(2, 34).Value = spoor903_905;
                worksheet.Cell(2, 35).Value = spoor903_906;
                worksheet.Cell(2, 36).Value = spoor905_910Z;
                worksheet.Cell(2, 37).Value = spoor905_C;
                worksheet.Cell(2, 38).Value = spoor906_B;
                worksheet.Cell(2, 39).Value = spoor906_908;
                worksheet.Cell(2, 40).Value = spoor908_A;
                worksheet.Cell(2, 41).Value = spoor908_X;
                worksheet.Cell(2, 42).Value = spoor910_D;
                worksheet.Cell(2, 43).Value = spoor909_G;

            workbook.SaveAs(@"C:\Users\akaraka1\source\repos\assenteller\event.xlsx");

        }
    }
}
