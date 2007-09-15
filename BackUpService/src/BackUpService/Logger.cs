using System;
using System.IO;

namespace BackUpService
{
  public class Logger
  {
    private static readonly object LOCK = new object();

    public static void Log(Exception e)
    {
      lock(LOCK) {
         File.AppendAllText(Config.Instance.LogFile, e.ToString());
      }
    }

    public static void LogMessage(string msg, params object[] args)
    {
      string date = DateTime.Now + ": ";
      File.AppendAllText(Config.Instance.LogFile, date + string.Format(msg, args) + Environment.NewLine);
      Console.Out.Write(date);
      Console.Out.WriteLine(msg, args);
    }
  }
}