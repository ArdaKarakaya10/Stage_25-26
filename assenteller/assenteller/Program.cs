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

// wisselstand sensor 1 is links en 0 rechts 
class Program
{
    static async Task Main()
    {
        var apiHandler = new ApiWebsite();
        var dbHandler = new DatabaseHandler();
        var excelHandler = new ExcelHandler();
        var Axleshandler = new RailAxles();

        var sensorData =  apiHandler.GetSensorDataAsync();

        var wpsData = dbHandler.GetWpsData();

        var AxlesData = Axleshandler.GetOldToNewList(wpsData);
        
        excelHandler.Export(sensorData, wpsData, AxlesData);

        Console.WriteLine("Export klaar!");


 
    }
    }
