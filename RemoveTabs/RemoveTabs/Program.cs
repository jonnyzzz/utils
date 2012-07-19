using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveTabs
{
  class Program
  {
    public static IEnumerable<string> ListAllFiles(string path)
    {
      return Directory.GetFiles(path).Union(Directory.GetDirectories(path).SelectMany(ListAllFiles));
    }

    static int Main(string[] args)
    {
      Console.Out.WriteLine("Processes all files replacing \\t into '  '.");
      Console.Out.WriteLine("Usage:");
      Console.Out.WriteLine(" <program> dir <extensions to process, i.e. .foo>");
      Console.Out.WriteLine("");

      var path = args.ElementAtOrDefault(0) ?? ".";
      var extensions = args.Skip(1).ToArray();
      if (!extensions.Any())
      {
        Console.Out.WriteLine("No masks were specified. Exit");
        return 1;
      }

      if (!Directory.Exists(path))
      {
        Console.Out.WriteLine("Directory not found: {0}", path);
      }

      Console.Out.WriteLine("Processing files under: {0}, mask: {1}", path, string.Join(", ", extensions));

      var allFiles = ListAllFiles(path)
        .Where(x => extensions.Any(e => x.EndsWith(e, StringComparison.InvariantCultureIgnoreCase)))
        .ToArray();

      if (!allFiles.Any())
      {
        Console.Out.WriteLine("Failed to find files.");
        return 1;
      }

      allFiles.AsParallel().ForAll(ProcessFile);
      Console.Out.WriteLine("");
      Console.Out.WriteLine("Processes {0} file(s)", allFiles.Count());

      return 0;
    }

    private static void ProcessFile(string file)
    {
      string oldText = File.ReadAllText(file);
      var newText = oldText.Replace("\t", "  ");
      if (oldText == newText) return;
      
      Console.Out.WriteLine("Updated: {0}", file);
      File.WriteAllText(file, newText);
    }
  }
}
