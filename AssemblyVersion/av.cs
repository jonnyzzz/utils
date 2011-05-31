using System;
using System.IO;
using System.Reflection;

namespace ConsoleApplication4
{
  class Program
  {
    static int Main(string[] args)
    {
      string file = args[0];
      if (!File.Exists(file))
      {
        Console.Out.WriteLine("File {0} is not found", file);
        return -1;
      }

      var info = Assembly.ReflectionOnlyLoadFrom(file);

      Console.Out.WriteLine("Assembly:        " + info.FullName);
      Console.Out.WriteLine("Runtime Version: " + info.ImageRuntimeVersion);

      Console.Out.WriteLine("References: ");
      foreach (var r in info.GetReferencedAssemblies())
      {
        Console.Out.WriteLine("  " + r.FullName);        
      }

      return 0;
    }
  }
}
