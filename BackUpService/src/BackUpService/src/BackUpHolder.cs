using System.Collections.Generic;

namespace BackUpService
{
  public class BackUpHolder
  {
    private readonly List<BackUpThreadBase> myServices = new List<BackUpThreadBase>();

    public void Start()
    {
      Config instance = Config.Instance;

      BackUpAction myBackupAction = new BackUpAction();
      BackupUploader backupUploader = new BackupUploader(instance.Upload);
      myBackupAction.OnArtifact += backupUploader.PublishBackup;

      foreach (Time up in instance.BackUps)
      {
        BackUpSleepThread item = new BackUpSleepThread(up.Hour, up.Minute);
        item.Time += myBackupAction.Action;
        item.Start();
        myServices.Add(item);
      }

      if (Config.Instance.Wait != null)
      {
        BackupUploader delayedUploader = new BackupUploader(instance.DelayedUpload);
        UploadWaitBackUpSleepThread th = new UploadWaitBackUpSleepThread(Config.Instance.DelayedUploadQueueLimit);

        delayedUploader.OnArtifact += th.ArtifactUploaded;
        backupUploader.OnArtifact += th.ArtifactReady;

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