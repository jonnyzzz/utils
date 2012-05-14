using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.TeamCity.Utils.PE;

namespace ConsoleApplication4
{
  class Program
  {
    static int Main(string[] args)
    {
      string file = args[0];
      if (Directory.Exists(file)) {
        foreach(var f in Directory.GetFiles(file, "*.*")) {
          if (f.EndsWith(".exe") || f.EndsWith(".dll")) {
            ProcessFile(f);
            Console.Out.WriteLine();
            Console.Out.WriteLine();
          }
        }
        return 0;
      }


      if (!File.Exists(file))
      {
        Console.Out.WriteLine("File {0} is not found", file);
        return -1;
      }

      ProcessFile(file);
     
      return 0;
    }

    static void ProcessFile(string file) {
      var info = Assembly.ReflectionOnlyLoadFrom(file);

      Console.Out.WriteLine("Assembly:        " + info.FullName);
      Console.Out.WriteLine("Runtime Version: " + info.ImageRuntimeVersion);
      Console.Out.WriteLine("Platform: "        + PEReader.DescribeAssemblyRuntime(file));

      Console.Out.WriteLine("References: ");

      var refs = info.GetReferencedAssemblies().Select(r => r.FullName + ", platform=" + r.ProcessorArchitecture).OrderBy(x=>x);
      foreach (var r in refs)
      {
        Console.Out.WriteLine("  " + r);
      }
    }
  }
}
