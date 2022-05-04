using System.IO;
using System.Text;
using Looxid.Link;
using System;

public class EEGCSVRecorder
{
    private const string EXTENSION = ".csv";
    private string folderName = "EEGVR";
    private string filepath;
    StreamWriter dataWriter;
    StreamWriter eventWriter;

    public void CreateFile(string fileName)
    {
        if (fileName == null) return;

        filepath = DataModuleBase.GetPath(User.Id);
        filepath = Path.Combine(filepath, folderName);

        Directory.CreateDirectory(filepath);
        dataWriter = new StreamWriter(Path.Combine(filepath, fileName + EXTENSION), true, Encoding.UTF8);

        eventWriter = new StreamWriter(Path.Combine(filepath, "Events" + EXTENSION), true, Encoding.UTF8);
        Header(dataWriter);
    }

    public void Write(EEGRawSignal rawSignalData)
    {
        if (dataWriter != null)
        {
            foreach (EEGRawSignalData data in rawSignalData.rawSignal)
            {
                DateTimeOffset dtOffset = DateTimeOffset.FromUnixTimeSeconds((long)data.timestamp);
                DateTime currentDt = dtOffset.UtcDateTime.ToLocalTime();
                string hms = DateTime.Now.ToString(("HH:mm:ss:fff"));
                //string hms = currentDt.ToString("HH-mm-ss");
                dataWriter.Write(hms+",");
                int index = 0;

                foreach (double d in data.ch_data)
                {
                    dataWriter.Write("" + d + ",");

                    if (index >= 5) break;
                    index++;
                }
                dataWriter.WriteLine();
            }
        }
    }

    public void WriteEvent(string id, float appSeconds) 
    {
        if (eventWriter != null)
        {
            eventWriter.WriteLine($"{id},{appSeconds},{DateTime.Now.ToString("HH:mm:ss:fff")}");
        }
    }

    public void CloseFile()
    {
        if (dataWriter != null) 
        {
            dataWriter.Close();
            eventWriter.Close();
        }
    }
    private void Header(StreamWriter writer)
    {
        writer.Write("Timestamp,");
        writer.Write("AF3, AF4, Fp1, Fp2, AF7, AF8,");
        writer.WriteLine();
    }
}