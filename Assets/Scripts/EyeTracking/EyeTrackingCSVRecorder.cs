using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

public class EyeTrackingCSVRecorder
{
    private const string EXTENSION = ".csv";
    private string folderName = "EyeTracking";
    private string fileName = "EyeData";
    private string filepath;
    StreamWriter dataWriter;

    public void CreateFile()
    {
        filepath = DataModuleBase.GetPath(User.Id);
        filepath = Path.Combine(filepath, folderName);

        Directory.CreateDirectory(filepath);
        dataWriter = new StreamWriter(Path.Combine(filepath, fileName + EXTENSION), true, Encoding.UTF8);

        Header(dataWriter);
    }

    public void Write(Dictionary<int, (string, int, float)> data)
    {
        if (dataWriter != null)
        {
            foreach (KeyValuePair<int, (string, int, float)> pair in data)
            {
                dataWriter.Write($"{pair.Value.Item1},{pair.Value.Item2},{GenTimeSpanFromSeconds(pair.Value.Item3)}");
                dataWriter.WriteLine();
            }
        }
    }

    //GenTimeSpanFromSeconds(pair.Value.Item3)
    public void CloseFile()
    {
        dataWriter.Close();
    }

    private void Header(StreamWriter writer)
    {
        //writer.Write("Timestamp,");
        writer.Write("Point Of Interest, Number of Looks, Total Stare Time,");
        writer.WriteLine();
    }

    static string GenTimeSpanFromSeconds(float timer)
    {
        TimeSpan t = TimeSpan.FromSeconds(timer);

        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);

        return answer;
    }
}