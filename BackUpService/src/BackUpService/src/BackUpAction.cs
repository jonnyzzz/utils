using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace BackUpService
{
  public static class BackUpAction
  {
    private const long K = 1024;
    private const long M = K*K;
    private const long RESERVED_SPACE = 300*M;

    private static readonly object LOCK = new object();

    private static string GetTmpDir()
    {
      return Path.Combine(TempFolder, "backup_" +DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    private static string TempFolder
    {
      get { return Config.Instance.TempFolder; }
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

    private static bool CheckSpace(string folder, long space)
    {
      try
      {
        DirectoryInfo di = new DirectoryInfo(folder);
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
          if (di.FullName.StartsWith(drive.RootDirectory.FullName, StringComparison.CurrentCultureIgnoreCase))
          {
            return drive.AvailableFreeSpace - RESERVED_SPACE> space;
          }
        }
        return true;
      } catch(Exception e)
      {
        Logger.Log(e);
        return true;
      }
    }

    private static long DirectorySize(string dir)
    {
      long size = 0;
      foreach (string childDir in Directory.GetDirectories(dir))
      {
        size += DirectorySize(childDir);
      }

      foreach (string file in Directory.GetFiles(dir))
      {
        size += new FileInfo(file).Length;
      }
      return size;      
    }

    private static void PublishBackup(string file)
    {
      long size = new FileInfo(file).Length;


      foreach (string upload in Config.Instance.Upload)
      {
        if (!CheckSpace(upload, size * 3))
        {
          RemoveOlderEntries(upload, size * 5);
        }

        try
        {
          Copy(file, Path.Combine(upload, Path.GetFileName(file)));
        } catch(Exception e)
        {
          Logger.Log(e);
          continue;          
        }
      }
    }

    private static void RemoveOlderEntries(string dir, long sizeToHave)
    {
      List<Pair<DateTime, string>> files = new List<Pair<DateTime, string>>();
      foreach (string file in Directory.GetFiles(dir))
      {
        if (new FileInfo(file).Length > 20 * M)
        {
          files.Add(Pair.Create(File.GetLastWriteTime(file), file));
        }
      }

      files.Sort(Comparison);

      long size = 0;
      foreach (Pair<DateTime, string> file in files)
      {
        Logger.LogMessage("Delete {0} to free space", file);
        FileInfo info = new FileInfo(file.B);
        size += info.Length;
        info.Delete();

        if (size > sizeToHave)
          break;
      }      
    }

    public static int Comparison(Pair<DateTime, string> f1, Pair<DateTime, string> f2)
    {
      DateTime d1 = f1.A;
      DateTime d2 = f2.A;

      return d1.CompareTo(d2);      
    }

    private static void Copy(string file, string dest)
    {
      File.Copy(file, dest, true);
    }
  }
}