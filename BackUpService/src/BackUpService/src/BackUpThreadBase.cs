using System;
using System.Threading;

namespace BackUpService
{
  public abstract class BackUpThreadBase
  {
    protected volatile bool myRunning = true;
    protected readonly Thread myThread;
    public event EventHandler<EventArgs> Time;

    protected BackUpThreadBase()
    {
      myThread = new Thread(Do);
      myThread.Name = string.Format("Alarm {0}", GetType().Name);
    }

    public void Start()
    {
      myThread.Start();
    }

    protected void Fire()
    {
      if (Time != null)
      {
        Time(this, EventArgs.Empty);
      }
    }

    protected abstract void Do();

    public void Stop()
    {
      myRunning = false;
      myThread.Interrupt();
      myThread.Join();
    }
  }
}