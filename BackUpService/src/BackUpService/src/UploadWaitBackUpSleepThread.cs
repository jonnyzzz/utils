using System;
using System.IO;
using System.Threading;

namespace BackUpService
{
  public class UploadWaitBackUpSleepThread : BackUpThreadBase
  {
    private static bool Check()
    {
      string[] wait = Config.Instance.Wait;
      if (wait == null)
        return true;

      foreach (string dir in wait)
      {
        if (Directory.Exists(dir))
          return true;
      }
      return true;
    }

    protected override void Do()
    {
      while (myRunning)
      {
        try
        {
          WaitInternal();
          try
          {
            Logger.LogMessage("Resource. Backup started.");
            Fire();
          }
          catch (Exception e)
          {
            Logger.Log(e);
          }
          finally
          {
            Logger.LogMessage("Resource. Backup finished.");
            if (Config.Instance.WaitSleep != null)
            {
              Thread.Sleep(new TimeSpan(Config.Instance.WaitSleep.Hour, Config.Instance.WaitSleep.Minute, 0));
            }
            else
            {
              Thread.Sleep(new TimeSpan(17, 2, 0));
            }
          }
        }
        catch (ThreadInterruptedException)
        {
        }
      }
    }


    protected static void WaitInternal()
    {
      while (true)
      {
        try
        {
          if (Check())
            return;
        }
        catch
        {
          Thread.Sleep(new TimeSpan(0, 10, 0));
          continue;
        }
      }
    }
  }
}