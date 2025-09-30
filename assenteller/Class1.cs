using System;

public class DeviceRecord
{
    public DateTime Received { get; set; }
    public string DevEUI { get; set; }
    public string Payload { get; set; }
    public string IO { get; set; }
    public string Temperature { get; set; }
    public string Battery { get; set; }
    public int CountUp { get; set; }
    public int CountDown { get; set; }
    public int Type { get; set; }
}

public class ApiResponse
{
    public List<DeviceRecord> Data { get; set; }
    public int Total { get; set; }
    public object AggregateResults { get; set; }
    public object Errors { get; set; }
}

