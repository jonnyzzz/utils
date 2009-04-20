using System;
using System.IO;

namespace EugenePetrenko.BibParser.Reader
{
  public class BibCommentRemover : TextReader
  {
    private readonly TextReader myReader;
    private char[] myLine;
    private int myIndex;

    public BibCommentRemover(TextReader reader)
    {
      myReader = reader;
      FillBuffer();
    }

    private void FillBuffer()
    {
      var str = myReader.ReadLine();
      if (str == null)
      {
        myLine = null;
        myIndex = -1;
      }
      else
      {
        str = str.Split('%')[0] + Environment.NewLine;
        myLine = str.ToCharArray();
        myIndex = 0;
      }
    }

    public override int Peek()
    {
      return myLine != null ? myLine[myIndex] : -1;
    }

    public override int Read()
    {
      if (myLine == null)
        return -1;
      var read = myLine[myIndex++];
      if (myIndex >= myLine.Length)
        FillBuffer();
      return read;
    }
  }
}