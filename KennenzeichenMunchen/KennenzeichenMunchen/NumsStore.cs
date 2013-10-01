using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KennenzeichenMunchen
{
  public class NumsStore
  {
    private static string Home(params string[] fs)
    {
      return fs.Aggregate(@"e:\nums\", Path.Combine);
    }

    public static void AddMoreNumbers(IEnumerable<Numb> numbers)
    {
      var data = new List<Numb>(numbers);
      data.Sort();

      string @join = String.Join("\r\n", data);
      File.WriteAllText(Home(@"nums-" + DateTime.Now.Ticks + ".txt"), @join);
    }

    public static IEnumerable<Numb> LoadNumbers()
    {
      var rex = new Regex(@"M\s*\w{2}\s*\d{4}", RegexOptions.Compiled);
      var allFiles = Directory.GetFiles(Home(), "*.txt").ToArray();

      var lines = allFiles
        .AsParallel()
        .SelectMany(File.ReadAllLines)
        .AsParallel()
        .Select(x => x.Trim())
        .Where(x => x.Length > 0)
        .AsParallel()
        .Distinct()
        .ToArray();

      Console.Out.WriteLine("There are {0} lines to process...", lines.Count());

      var data = lines
        .AsParallel()
        .Select(x => rex.Match(x))
        .Where(x => x.Success)
        .Select(x => x.Groups[0].Value)
        .Where(x => !string.IsNullOrEmpty(x))
        .AsParallel()
        .Select(x => new Numb(x))
        .ToList();

      Console.Out.WriteLine("There are {0} Numbs to created...", data.Count());

      data.Sort();
      data = data.Distinct(NumbEqualityComparer.THIS).ToList();

      Console.Out.WriteLine("Total Numbs: {0}", data.Count());

      AddMoreNumbers(data);

      allFiles.AsParallel().ForAll(x=>File.Move(x, x + ".old"));
      return data;
    }
  }
}
