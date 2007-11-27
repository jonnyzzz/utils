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
        if (Path.GetFileName(directory) == ".svn")
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

      List<string> entries = new List<string>();      
      EnumerateEntriesFiles(sourceRep, entries);

      foreach (string file in entries)
      {
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

