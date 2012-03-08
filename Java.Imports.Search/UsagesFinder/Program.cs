using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace UsagesFinder
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Out.WriteLine("Java Usages scanner. ");
      Console.Out.WriteLine("Java.Usages.exe <base directory> <namespace to search>");

      string baseDir = args[0];
      string namespaceName = args[1];

      Console.Out.WriteLine("Listing classes: ");
      var allClasses = new[]
                         {                           
                           @"S:\Mount\TeamCity\trunk\buildserver\tools\idea\lib\util.jar",
                           @"S:\Mount\TeamCity\trunk\buildserver\tools\idea\lib\openapi.jar",
                         }.SelectMany(ListClassNames).ToArray().ToLookup(x=>x);

      Console.Out.WriteLine("Found {0} classes", allClasses.Count());

      Console.Out.WriteLine("");
      Console.Out.WriteLine("Scanning {0} for {1}...", baseDir, namespaceName);

      Func<IEnumerable<string>, IEnumerable<string>> proFiles = 
        files => files.Select(x => x.Substring(baseDir.Length + 1).Split(new [] {"\\src\\"}, StringSplitOptions.None)[0])          
          .OrderBy(x => x)
          .Distinct()
          .ToArray();

      Func<string, IEnumerable<string>> replaceWildcard = name
                                                          =>
                                                            {
                                                              if (!name.Contains("*")) return new[] {name};
                                                              name = name.Replace("*", "");
                                                              return
                                                                allClasses.Where(x => x.Key.StartsWith(name)).Select(
                                                                  x => x.Key).ToArray();
                                                            };

      var usages = ListAllFiles(baseDir)
        .AsParallel()
        .Where(x => !x.Contains("testData"))
        .Where(x => x.Contains("\\src\\"))
        .Where(x => !x.Contains("build-server4idea"))
        .SelectMany(x => FindUsagesInFile(x, namespaceName).SelectMany(replaceWildcard).Select(y => new {File = x, Namespace = y}))
        .Where(x => allClasses.Contains(x.Namespace) || x.Namespace.Contains("*"))
        .Distinct();

      var usageByClass = usages 
        .GroupBy(x=>x.Namespace, (key, items) => new {Namespace = key, Count = items.Count(), Files = proFiles(items.Select(x=>x.File)).ToArray()})
        .OrderBy(x=>x.Namespace)
        .ToArray();

      var usageByModule = usageByClass
        .SelectMany(x => x.Files.Select(y => new {x.Namespace, File = y}))
        .GroupBy(x => x.File, (key, items) => new {File = key, Count = items.Count(), Classes = items.Select(f => f.Namespace).Distinct().ToArray()})
        .ToArray();

      Console.Out.WriteLine("h3. Total usages: {0}", usageByClass.Count());
      foreach (var usage in usageByClass)
      {
        Console.Out.WriteLine("- {{{{{0}}}}},  modules: {1}", usage.Namespace, usage.Count);
      }
      Console.Out.WriteLine();
      Console.Out.WriteLine();
      Console.Out.WriteLine("h3. Found {0} usages:", usageByClass.Count());
      foreach (var usage in usageByClass)
      {
        Console.Out.WriteLine("- {{{{{0}}}}}, modules: {1}", usage.Namespace, usage.Count);
        foreach (var file in usage.Files)
        {
          Console.Out.WriteLine("-- {{{{{0}}}}}", file);
        }       
      }

      Console.Out.WriteLine("");
      Console.Out.WriteLine("h3. Usage per module:");
      foreach (var file in usageByModule)
      {
        Console.Out.WriteLine("- {{{{{0}}}}}, classes: {1}", file.File, file.Count);
        foreach (var clz in file.Classes)
        {
          Console.Out.WriteLine("-- {{{{{0}}}}}", clz);
        }
      }
    }


    private static IEnumerable<string> ListClassNames(string jarFile)
    {
      using(var stream = new FileStream(jarFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using(var zf = new ZipInputStream(stream))
      {
        var names = new List<string>();
        ZipEntry e;
        while((e = zf.GetNextEntry()) != null)
        {
          names.Add(e.Name);
        }

        return names
          .Where(x => x.EndsWith(".class"))          
          .Select(x => x.Substring(0, x.Length - 6))
          .Select(x => x.TrimStart("\\/".ToCharArray()))
          .Select(x => x.Replace('/', '.'))
          .Select(x => x.Replace('\\', '.'))
          .Select(x => x.Replace('$', '.'))
          .ToArray();
      }
    } 


    private static IEnumerable<string> FindUsagesInFile(string file, string namespaceName)
    {
      return File.ReadAllLines(file)
        .Select(x => x.Trim())
        .Where(x => x.StartsWith("import ") && x.EndsWith(";"))
        .Select(x => x.Substring("import".Length).TrimEnd(";".ToCharArray()).Trim())
        .Where(x => x.StartsWith(namespaceName))        
        ;
    } 


    private static IEnumerable<string> ListAllFiles(string baseDir)
    {
      return Directory.GetFiles(baseDir, "*.java").Union(Directory.GetDirectories(baseDir).SelectMany(ListAllFiles));
    }
  }
}
