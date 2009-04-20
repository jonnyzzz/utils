using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EugenePetrenko.BibParser.AuxParser;
using EugenePetrenko.BibParser.Reader;

namespace BibTexE
{
  public class Program
  {
    private static readonly Encoding ENCODING = Encoding.GetEncoding("windows-1251");

    public static int Main(string[] args)
    {
      if (args.Length != 3)
      {
        return Usage();
      }
      string bibFile = args[0];
      string auxFile = args[1];
      string dest = args[2];

      if (!File.Exists(bibFile))
      {
        Console.Out.WriteLine("File {0} does not exist", bibFile);
        return Usage();
      }
      if (!File.Exists(auxFile))
      {
        Console.Out.WriteLine("File {0} does not exist", auxFile);
        return Usage();
      }

      var refs = new HashSet<string>();
      using(TextReader tw = new StreamReader(auxFile, ENCODING))
        refs.UnionWith(new AuxReader(tw).ReadRefs());

      Console.Out.WriteLine("Used {0} references.", refs.Count);

      var bibRecords = new List<RawRecord>();
      using(TextReader tw = new StreamReader(bibFile, ENCODING))
      {
        bibRecords.AddRange(new BibReader(new BibLexer(new BibCommentRemover(tw))).Parse());
      }

      Console.Out.WriteLine("Found {0} definitions", bibRecords);

      bibRecords.RemoveAll(x => !refs.Contains(x.RefName));

      if (bibRecords.Count != refs.Count)
      {
        foreach (var rf in refs.Where(x=>bibRecords.Exists(xx=>xx.RefName == x)))
        {
          Console.Out.WriteLine("Undefined refence {0}", rf);
        }
      }

      //TODO: Generate code


      return 0;
    }

    private static int Usage()
    {
      Console.Out.WriteLine("<assembly> <.bib> <.aux> <dest.bbl>");
      return -1;
    }
  }
}
