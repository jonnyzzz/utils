using System;
using System.IO;
using System.Text;

namespace EugenePetrenko.Encodings {
  class MainClass {
    
    public static int Main(string[] args) {
      if (args.Length != 3 && args.Length != 4) {
        Usage();
        return -1;
      }

      var destFile = args.Length == 4 && args[3] != null ? args[3] : (args[2] + ".dest");

      try
      {
        Encoding source = Encoding.GetEncoding(args[0]);
        Encoding dest = Encoding.GetEncoding(args[1]);

        if (source == null) 
          throw new ArgumentException("source");

        if (dest == null) 
          throw new ArgumentException("dest");

        using (var s = new StreamReader(args[2], source))
        {
          
          using (var d = new FileStream(destFile, FileMode.Create, FileAccess.Write))
          {
            for (string c; ((c = s.ReadLine()) != null);)
            {
              Console.Out.WriteLine(">>{0}", c);
              var array = dest.GetBytes(c + Environment.NewLine);
              d.Write(array, 0, array.Length);
            }
          }
        }
        return 0;
      } catch (Exception e)
      {
        Console.Out.WriteLine("Failed with " + e.Message);
        return -1;
      }
    }

    private static void Usage() {
       Console.Out.WriteLine("<exe> source_encoding dest_encoding file");
    }
  }
}