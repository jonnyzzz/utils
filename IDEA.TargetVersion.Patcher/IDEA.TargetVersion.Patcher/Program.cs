using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace IDEA.TargetVersion.Patcher
{
  static class Program
  {
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

      var modules = new XmlDocument();
      modules.Load(Path.Combine(home, ".idea", "modules.xml"));
      var li = modules.SelectNodes("/project/component/modules/module/@filepath");
      if (li == null)
      {
        Console.Out.WriteLine("Failed to parse modules.xml");
        return;
      }

      var allFiles = li.Cast<XmlAttribute>().Select(x => x.Value.Replace("$PROJECT_DIR$", home)).Select(Path.GetFullPath).ToArray();
      Console.Out.WriteLine("Found: {0} module file(s)", allFiles.Count());

      var toPatch = new Dictionary<string, string>();
      foreach (var file in allFiles)
      {
        var doc = new XmlDocument();
        doc.Load(file);

        var node = doc.SelectSingleNode("/module/component[@name='NewModuleRootManager']/@LANGUAGE_LEVEL") as XmlAttribute;
        if (node == null) continue;
        var moduleName = Path.GetFileNameWithoutExtension(file);
        var value = node.Value;

        if (value == "JDK_1_3") value = "1.3";
        if (value == "JDK_1_4") value = "1.4";
        if (value == "JDK_1_5") value = "1.5";
        if (value == "JDK_1_6") value = "1.6";
        if (value == "JDK_1_7") value = "1.7";
        if (value == "JDK_1_8") value = "1.8";

        Console.Out.WriteLine("{1} => {0}", moduleName, value);        
        toPatch.Add(moduleName, value);
      }

      var compilerXml = Path.Combine(home, ".idea", "compiler.xml");

      var compiler = new XmlDocument();
      compiler.Load(compilerXml);

      var root = compiler.SelectSingleNode("/project/component[@name='CompilerConfiguration']/bytecodeTargetLevel") as XmlElement;
      root.RemoveAll("module");

      foreach (var m in toPatch.OrderBy(x=>x.Key))
      {
        var mm = root.OwnerDocument.CreateElement("module");
        mm.SetAttribute("name", m.Key);
        mm.SetAttribute("target", m.Value);
        root.AppendChild(mm);
      }

      File.SetAttributes(compilerXml, FileAttributes.Normal);
      using (var stream = new StreamWriter(File.Create(compilerXml), new UTF8Encoding(false)))
      {
        compiler.Save(stream);
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
