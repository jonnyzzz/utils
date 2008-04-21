using System;
using System.Collections.Generic;
using System.IO;

namespace BackUpService
{
  public class BackupUploader : ArtifactReadyBase
  {
    private const long K = 1024;
    private const long M = K*K;
    private const long RESERVED_SPACE = 300*M;

    private readonly ICollection<string> myUploadFiles;

    public BackupUploader(ICollection<string> myUploadFiles)
    {
      this.myUploadFiles = myUploadFiles;
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
            return drive.AvailableFreeSpace - RESERVED_SPACE > space;
          }
        }
        return true;
      }
      catch (Exception e)
      {
        Logger.LogMessage("Failed to CheckSpace on {0} with space {1}mb", folder, (double) space/M);
        Logger.Log(e);
        return true;
      }
    }

    public void PublishBackup(long fileId, string file, bool _crap)
    {
      long size = new FileInfo(file).Length;

      foreach (string upload in myUploadFiles)
      {
        if (!CheckSpace(upload, size*3))
        {
          RemoveOlderEntries(upload, size*5);
        }

        string dest = Path.Combine(upload, Path.GetFileName(file));
        try
        {
          Copy(file, dest);
        }
        catch (Exception e)
        {
          Logger.LogMessage("Failed to copy {0} to {1}", file, upload);
          Logger.Log(e);
          continue;
        }

        FireAtrifact(fileId, dest, false);
      }
    }

    private static void RemoveOlderEntries(string dir, long sizeToHave)
    {
      List<Pair<DateTime, string>> files = new List<Pair<DateTime, string>>();
      foreach (string file in Directory.GetFiles(dir))
      {
        if (new FileInfo(file).Length > 20*M)
        {
          files.Add(Pair.Create(File.GetLastWriteTime(file), file));
        }
      }

      files.Sort(Comparison);

      long size = 0;
      foreach (Pair<DateTime, string> file in files)
      {
        Logger.LogMessage("Delete {0} to free space", file.B);
        FileInfo info = new FileInfo(file.B);
        size += info.Length;
        info.Delete();

        if (size > sizeToHave)
          break;
      }
    }

    private static int Comparison(Pair<DateTime, string> f1, Pair<DateTime, string> f2)
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