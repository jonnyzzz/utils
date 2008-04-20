using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BackUpService
{
  public class UploadWaitBackUpSleepThread : BackUpThreadBase
  {
    private readonly AutoResetEvent myFilesReady = new AutoResetEvent(false);
    private readonly List<UploadInfo> myUploadQueue = new List<UploadInfo>();
    private readonly int myUploadLimit;

    public event ArtifactReady OnArtifactReady;

    public UploadWaitBackUpSleepThread(int myUploadLimit)
    {
      Time += UploadWaitBackUpSleepThread_Time;
      this.myUploadLimit = myUploadLimit;
    }

    private void UploadWaitBackUpSleepThread_Time(object sender, EventArgs e)
    {
      if (OnArtifactReady == null)
        return;
      ICollection<UploadInfo> files;
      lock (myUploadQueue)
      {
        files = new List<UploadInfo>(myUploadQueue);
      }

      foreach (UploadInfo info in files)
      {
        try
        {
          OnArtifactReady(info.Id, info.File, false);
        }
        catch
        {
          lock (myUploadQueue)
          {
            myUploadQueue.Remove(info);
          }
        }
      }
    }

    public void ArtifactReady(long fileId, string file, bool temp)
    {
      if (temp)
        return;

      lock (myUploadQueue)
      {
        UploadInfo info = new UploadInfo(fileId, file);
        if (myUploadQueue.Contains(info))
          return;

        if (myUploadQueue.Count >= myUploadLimit)
        {
          myUploadQueue.RemoveAt(0);
        }
        myUploadQueue.Add(info);

        myFilesReady.Set();
      }
    }

    public void ArtifactUploaded(long fileId, string file, bool temp)
    {
      lock (myUploadQueue)
      {
        UploadInfo info = new UploadInfo(fileId, file);
        if (myUploadQueue.Contains(info))
        {
          myUploadQueue.Remove(info);
        }
      }
    }

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
            Logger.LogMessage("Resource backup started uploader.");
            Fire();
          }
          catch (Exception e)
          {
            Logger.Log(e);
          }
          finally
          {
            Logger.LogMessage("Resource. Backup finished.");
          }
        }
        catch (ThreadInterruptedException)
        {
        }
      }
    }

    private void WaitInternal()
    {
      while (true)
      {
        bool waitEvent;
        lock (myUploadQueue)
        {
          waitEvent = myUploadQueue.Count == 0;
        }

        if (waitEvent)
        {
          myFilesReady.WaitOne();
        }

        try
        {
          if (Check())
            return;
        }
        catch
        {
          Thread.Sleep(new TimeSpan(0, 5, 0));
          continue;
        }
      }
    }

    private class UploadInfo : IEquatable<UploadInfo>
    {
      public readonly long Id;
      public readonly string File;

      public UploadInfo(long id, string file)
      {
        File = file;
        Id = id;
      }

      public bool Equals(UploadInfo uploadInfo)
      {
        if (uploadInfo == null) return false;
        return Id == uploadInfo.Id;
      }

      public override bool Equals(object obj)
      {
        return ReferenceEquals(this, obj) || Equals(obj as UploadInfo);
      }

      public override int GetHashCode()
      {
        return (int) Id;
      }
    }
  }
}