using  System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace BackUpService
{
  public static class BackUpAction
  {
    private static readonly object LOCK = new object();

    private static string GetTmpDir()
    {
      return Path.Combine(Config.Instance.TempFolder, "backup_" +DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    public static void Action(object sender, EventArgs args)
    {
      lock(LOCK) {
 
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
        PublishBackup(backUp);

        File.Delete(backUp);
      } finally
      {                
        Directory.Delete(tmpFile);
        Directory.Delete(tmpFileOutput);
      }
      }
    }

    private static void PublishBackup(string file)
    {
      foreach (string upload in Config.Instance.Upload)
      {
        try
        {
          Copy(file, Path.Combine(upload, Path.GetFileName(file)));
        } catch
        {
          continue;          
        }
      }
    }

    private static void Copy(string file, string dest)
    {
      File.Copy(file, dest, true);
    }
  }
}