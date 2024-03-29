using System;
using System.Collections.Generic;
using System.IO;
using EugenePetrenko.BibParser.Util;

namespace EugenePetrenko.BibParser.Biblio
{
  public class BiblioWriter : IDisposable
  {
    private readonly TextWriter myWriter;
    private readonly List<Pair<string, string>> myBiblio = new List<Pair<string, string>>();

    public BiblioWriter(TextWriter writer)
    {
      myWriter = writer;
    }

    public void WriteBibitem(string name, string text)
    {
      myBiblio.Add(Pair.Of(name, text));
    }

    public void Dispose()
    {
      int count = myBiblio.Count;
      myWriter.WriteLine("%Generated by Eugene Petrenko");
      myWriter.WriteLine("%This file is generated automatically at {0}", DateTime.Now);
      myWriter.WriteLine();

      myWriter.WriteLine(@"\begin{thebibliography}{" + count + "}");

      foreach (var pair in myBiblio)
      {
        myWriter.Write("   ");
        myWriter.Write(@"\bibitem{");
        myWriter.Write(pair.First);
        myWriter.WriteLine("}");
        myWriter.WriteLine(pair.Second);
        myWriter.WriteLine();
      }

      myWriter.WriteLine(@"\end{thebibliography}");
    }
  }
}