using System;
using System.IO;
using System.Xml.Serialization;

namespace BackUpService
{
  [Serializable]
  [XmlRoot("config")]
  public class Config
  {
    private static Config myInstance;

    public static Config Instance
    {
      get
      {
        if (myInstance == null)
        {
          myInstance = Load();
        }
        return myInstance;
      }
    }

    public Config()
    {
    }

    [XmlArray("times")] [XmlArrayItem("time")] public Time[] BackUps;

    [XmlElement("logFile")] public string LogFile;

    [XmlArray("watches")] [XmlArrayItem("watch")] public string[] BackupFolders;

    [XmlElement("temp")] public string TempFolder;

    [XmlElement("delayed-upload-limit")] public int DelayedUploadQueueLimit;

    [XmlArray("Uploads")] [XmlArrayItem("upload")] public string[] Upload;

    [XmlArray("delayed-uploads")] [XmlArrayItem("upload")] public string[] DelayedUpload;

    [XmlArray("Waits")] [XmlArrayItem("dir")] public string[] Wait;
    [XmlElement("WaitSleep")] public Time WaitSleep;


    private static Config Load()
    {
      XmlSerializer ser = new XmlSerializer(typeof (Config));
      string path = Path.Combine(Path.GetDirectoryName(typeof (Config).Assembly.Location), "config.xml");
      if (File.Exists(path))
      {
        using (Stream s = File.OpenRead(path))
        {
          Config cfg = (Config) ser.Deserialize(s);
          return cfg;
        }
      }
      Config tmp = new Config();
      tmp.BackupFolders = new string[] {"backup folder"};
      tmp.BackUps = new Time[] {new Time(0, 0),};
      tmp.LogFile = "log file";
      tmp.TempFolder = "tempFolder";
      tmp.Upload = new string[] {"upload"};
      tmp.WaitSleep = new Time(0, 0);
      tmp.Wait = new string[] {"will wait till resource available"};
      tmp.DelayedUpload = new string[] {"delayed uploads here"};
      using (Stream s = File.Create(path + ".example"))
        ser.Serialize(s, tmp);

      throw new Exception("Unable to locate config.xml at " + path);
    }
  }
}