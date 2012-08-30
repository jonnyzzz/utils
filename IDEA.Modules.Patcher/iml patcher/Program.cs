using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace iml_patcher
{
  static class Program
  {
    private static IEnumerable<string> ListAllFiles(string baseDir, string mask)
    {
      return Directory.GetFiles(baseDir, mask).Union(Directory.GetDirectories(baseDir).SelectMany(x=>ListAllFiles(x, mask)));
    }

    private static IEnumerable<string> ListAllFilesAndDirs(string baseDir)
    {
      var dirs = Directory.GetDirectories(baseDir);
      return Directory.GetFiles(baseDir).Union(dirs).Union(dirs.SelectMany(ListAllFilesAndDirs));
    }

    static void Main(string[] args)
    {
      Console.Out.WriteLine("Usage");
      Console.Out.WriteLine(" app.exe <path to IDEA project root>");

      var home = args.ElementAtOrDefault(0);
      if (home == null || !Directory.Exists(home))
      {
        Console.Out.WriteLine("Failed to get IDEA project root directory");
        return;
      }

      Console.Out.WriteLine("Removing obsolete output directories...");
      foreach (var dir in ListAllFilesAndDirs(home).ToArray())
      {
        if (Directory.Exists(dir) && (dir.ToLower().EndsWith("classes") || dir.ToLower().EndsWith("testclasses")))
          Directory.Delete(dir, true);
      }

      var modules = new XmlDocument();
      modules.Load(Path.Combine(home, ".idea", "modules.xml"));
      var li = modules.SelectNodes("/project/component/modules/module/@filepath");
      if (li == null)
      {
        Console.Out.WriteLine("Failed to parse modules.xml");
        return;
      }
      
      var allFiles = 
        li.Cast<XmlAttribute>().Select(x => x.Value.Replace("$PROJECT_DIR$", home)).Select(Path.GetFullPath).ToArray();

      Console.Out.WriteLine("Found: {0} module file(s)", allFiles.Count());

      foreach (var file in allFiles)
      {
        var doc = new XmlDocument();
        doc.Load(file);

        var node = doc.SelectSingleNode("/module/component[@name='NewModuleRootManager']") as XmlElement;
        if (node == null) continue;

        node.SetAttribute("inherit-compiler-output", "true");
        node.RemoveAll("output", "exclude-output", "output-test");

        var jdk = node.SelectSingleNode("orderEntry[@type='jdk']") as XmlElement;
        if (jdk != null)
          Console.Out.WriteLine("Module: {0}, JDK: {1}", Path.GetFileNameWithoutExtension(file), jdk.OuterXml);

        using (var stream = new StreamWriter(File.Create(file), new UTF8Encoding(false)))
        {
          doc.Save(stream);
        }
      }
    }

    public static void RemoveAll(this XmlElement element, params string[] names)
    {
      var toRemove = names
        .Select(element.SelectNodes)
        .Where(x => x != null)
        .SelectMany(x => x.Cast<XmlElement>())
        .ToArray();

      foreach (var el in toRemove)
      {
        XmlNode node = el.ParentNode;
        if (node != null) node.RemoveChild(el);
      }
    }
  }


}
