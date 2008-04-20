using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

namespace BackUpService
{
  public class BackUpAction : ArtifactReadyBase
  {
    private static readonly object LOCK = new object();
    private long myFileId = 0;

    private static string GetTmpDir()
    {
      return Path.Combine(TempFolder, "backup_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    private static string TempFolder
    {
      get { return Config.Instance.TempFolder; }
    }

    public void Action(object sender, EventArgs args)
    {
      lock (LOCK)
      {
        long id = Interlocked.Increment(ref myFileId);

        string tmpFile = GetTmpDir();
        if (!Directory.Exists(tmpFile))
          Directory.CreateDirectory(tmpFile);

        string tmpFileOutput = tmpFile + "_output";
        if (!Directory.Exists(tmpFileOutput))
          Directory.CreateDirectory(tmpFileOutput);

        try
        {
          Logger.LogMessage("Begin backing up");

          FastZip zip = new FastZip();
          zip.CreateEmptyDirectories = true;

          List<string> zipFiles = new List<string>();
          foreach (string folder in Config.Instance.BackupFolders)
          {
            string zipFile = Path.Combine(tmpFile, Path.GetFileName(folder) + ".zip");
            zip.CreateZip(zipFile, folder, true, null);
            zipFiles.Add(zipFile);
          }
          string backUp = Path.Combine(tmpFileOutput, "backup_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".zip");

          Logger.LogMessage("Create Backup file");

          FastZip z = new FastZip();
          z.CreateZip(backUp, tmpFile, true, null);

          foreach (string file in zipFiles)
          {
            File.Delete(file);
          }

          Logger.LogMessage("Publishing backup");
          FireAtrifact(id, backUp, true);

          File.Delete(backUp);
        }
        finally
        {
          Directory.Delete(tmpFile);
          Directory.Delete(tmpFileOutput);
        }
      }
    }
  }
}