using System;
using System.Threading;

namespace BackUpService
{
  public class BackUpSleepThread : BackUpThreadBase
  {
    private readonly int myHour;
    private readonly int myMinute;

    public BackUpSleepThread(int h, int m)
    {
      myHour = h;
      myMinute = m;
    }

    private static int ModDist(int time, int value, int mod)
    {
      if (time < value)
        return value - time;
      if (Math.Abs(time - value) <= 1)
        return 0;
      return Math.Abs(mod - value + time - 1)%mod;
    }

    protected override void Do()
    {
      while (myRunning)
      {
        try
        {
          try
          {
            Logger.LogMessage("Time. Files Backup started.");
            Fire();
          }
          catch (Exception e)
          {
            Logger.Log(e);
          }
          finally
          {
            Logger.LogMessage("Time. Files Backup finished.");
            Thread.Sleep(new TimeSpan(0, 0, 59));
          }

          DateTime stop = DateTime.Now + new TimeSpan(myHour, myMinute, 0);

          Logger.LogMessage("Next time to backup: {0} (+ {1}h {2}m)", stop, myHour, myMinute);

          Thread.Sleep(new TimeSpan(myHour, myMinute, 0));
        }
        catch (ThreadInterruptedException)
        {
        }
      }
    }
  }
}