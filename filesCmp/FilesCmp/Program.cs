using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FilesCmp
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Out.WriteLine("fCmp.exe <dir1> <dir2> <sz threashold>");

      var l = args[0];
      var r = args[1];
      var szDiff = int.Parse(args[2]);

      foreach (var path in ListFiles(l).Union(ListFiles(r)).Distinct())
      {
        bool isLeft = File.Exists(l + path);
        bool isRight = File.Exists(r + path);

        if (isLeft && !isRight)
        {
          Console.Out.WriteLine("{0}. Left only", path);
          continue;
        }
        
        if (!isLeft && isRight)
        {
          Console.Out.WriteLine("{0}. Right only", path);
          continue;
        }

        Func<string, long> fsize = name =>
                                  {
                                    using (var sz = File.OpenRead(name)) return sz.Length;
                                  };

        var lsz = fsize(l + path);
        var rsz = fsize(r + path);

        if (Math.Abs(lsz - rsz) > szDiff * 1024)
        {
          Console.Out.WriteLine("{0}. Left size {1}, right size {2}", path, lsz, rsz);
        }
      }
    }

    private static IEnumerable<string> ListFiles(string path)
    {
      return ListFilesRec(path).Select(x=>x.Substring(path.Length)).ToList();        
    }

    private static IEnumerable<string> ListFilesRec(string path)
    {
      return Directory.GetFiles(path)
        .Union(Directory.GetDirectories(path).SelectMany(ListFilesRec));
    }
  }
}
