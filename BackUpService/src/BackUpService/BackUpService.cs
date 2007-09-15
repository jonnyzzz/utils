using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;

namespace BackUpService
{
  public partial class BackUpService : ServiceBase
  {
    private readonly BackUpHolder myHolder = new BackUpHolder();

    public BackUpService()
    {
      InitializeComponent();
    }

    protected override void OnStart(string[] args)
    {
      try
      {
        myHolder.Start();
      } catch(Exception e)
      {
        EventLog.WriteEntry("Failed to start, " + e, EventLogEntryType.Error);
        Stop();
      }
    }

    protected override void OnStop()
    {
      myHolder.Stop();
    }
  }
}
