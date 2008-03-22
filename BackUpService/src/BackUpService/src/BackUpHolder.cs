using System.Collections.Generic;

namespace BackUpService
{
  public class BackUpHolder
  {
    private readonly List<BackUpThreadBase> myServices = new List<BackUpThreadBase>();

    public void Start()
    {
      Config instance = Config.Instance;

      foreach (Time up in instance.BackUps)
      {
        BackUpSleepThread item = new BackUpSleepThread(up.Hour, up.Minute);
        item.Time += BackUpAction.Action;
        item.Start();
        myServices.Add(item);
      }
      if (Config.Instance.Wait != null)
      {
        BackUpThreadBase th = new UploadWaitBackUpSleepThread();
        th.Time += BackUpAction.Action;
        th.Start();
        myServices.Add(th);
      }
    }

    public void Stop()
    {
      foreach (BackUpThreadBase thread in myServices)
      {
        thread.Stop();
      }
    }
  }
}