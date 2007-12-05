using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ClassLibrary1
{
  public class Class1
  {
    public static void EnumerateEntriesFiles(string path, List<string> entries)
    {
      foreach (string directory in Directory.GetDirectories(path))
      {
        if (directory.EndsWith("\\.svn"))
        {
          string pth = Path.Combine(directory, "entries");
          if (File.Exists(pth))
            entries.Add(pth);
        } else
        {
          EnumerateEntriesFiles(directory, entries);
        }
      }
    }

    public static void Main(string[] args)
    {
      string fromUrl = args[0];
      string toUrl = args[1];
      string sourceRep;
      if (args.Length > 2)
        sourceRep = args[2];
      else
        sourceRep = Environment.CurrentDirectory;

      if (args.Length <= 1) {
         Console.Out.WriteLine("svnRepoMove.exe from to [path]");
         return;
      }

      List<string> entries = new List<string>();      
      EnumerateEntriesFiles(sourceRep, entries);

      foreach (string file in entries)
      {
        foreach(string old in Directory.GetFiles(Path.GetDirectoryName(file), "entries.tmp_*")) {
          File.SetAttributes(old, FileAttributes.Normal);
          File.Delete(old);
          Console.Out.WriteLine("Delete older temp file {0}", old);
        }

        File.Copy(file, file + ".tmp_" + DateTime.Now.ToString("dd.mm.yyyy-HH.MM.ss"));

        FileAttributes attrs = File.GetAttributes(file);
        File.SetAttributes(file, FileAttributes.Normal);

        string text;
        using (TextReader tw = File.OpenText(file))
          text = tw.ReadToEnd();

        text = text.Replace(fromUrl, toUrl);
        
        using(TextWriter tw = File.CreateText(file))
          tw.Write(text);

        File.SetAttributes(file, attrs);

        Console.Out.WriteLine("entries = {0}", file);
      }
      
    }
  }
}

