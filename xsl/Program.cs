using System;
using System.IO;
using System.Xml.Xsl;

namespace xsl
{
  class Program
  {
    static int Main(string[] args)
    {
      try
      {
        if (args.Length != 3)
        {
          Console.Out.WriteLine("Usage:\r\nxsl.exe xml xsl result\r\n");
          return -1;
        }

        aMain(args[0], args[1], args[2]);
        
        return 0;
      } catch (ErrorException e)
      {
        Console.WriteLine(e);
        return e.Code;
      } catch(Exception e)
      {
        Console.WriteLine(e);
        return -1;
      }      
    }

    static void aMain(string xml, string xsl, string result)
    {
      if (!File.Exists(xml))
        Error("xml file does not exists, {0}", xml);
      if (!File.Exists(xsl))
        Error("xsl file does not exists, {0}", xsl);


      XslCompiledTransform transform = new XslCompiledTransform(false);
      transform.Load(xsl);

      transform.Transform(xml, result);
    }

    static void Error(string message, params object[] ps)
    {
      Console.Error.Write("Error: {0}", string.Format(message, ps));
      throw new ErrorException(-1,"");
    }
  }

  class ErrorException : Exception
  {
    public readonly int Code;

    public ErrorException(int code, string mgs) : base(mgs)
    {
      Code = code;
    }
  }
}
