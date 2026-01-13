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
using System.Diagnostics;

// wisselstand sensor 1 is links en 0 rechts 
class Program
{
    static async Task Main()
    {
        Stopwatch stopwatch = new Stopwatch();

        // Start logging
        stopwatch.Start();
        Console.WriteLine($"Verwerking gestart om {DateTime.Now:HH:mm:ss.fff}");

        var apiHandler = new ApiWebsite();
        var dbHandler = new DatabaseHandler();
        var excelHandler = new ExcelHandler();
        var Axleshandler = new RailAxles();

        var sensorData =  apiHandler.GetSensorDataAsync();

        
        var wpsData = dbHandler.GetWpsData();

        var AxlesData = Axleshandler.GetOldToNewList(wpsData);

        excelHandler.Export(sensorData, wpsData, AxlesData);
        // Einde verwerking
        stopwatch.Stop();
        Console.WriteLine($"Verwerking afgerond om {DateTime.Now:HH:mm:ss.fff}");
        Console.WriteLine($"Totale verwerkingstijd: {stopwatch.Elapsed.TotalSeconds} seconden");
        Console.WriteLine("Export klaar!");


 
    }
    }
