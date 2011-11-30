using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Tomcat.MimeTypesPatcher
{
  class Program
  {
    static void Main(string[] args)
    {
      var allMimes = new Dictionary<string, string>();

      foreach (var file in new[] { @"E:\Work\TeamCity\trunk\mimes.old", @"E:\Work\TeamCity\trunk\buildserver\tools\tomcat\conf\web.xml"})
      {
        Merge(allMimes, ImportFromTomCatConfig(file));
      }
      
      Merge(allMimes, ParseApacheHTTPDMimes());
      Console.Out.WriteLine(Dump(allMimes));

    }

    public static void Merge(Dictionary<string, string> a, Dictionary<string, string> b)
    {
      foreach (var kv in b.Keys)
      {
        if (a.ContainsKey(kv) && a[kv] != b[kv])
        {
          Console.Out.WriteLine("Conflicting entry for: {0}->{1}:{0}->{2}", kv, a[kv], b[kv]);
        }
        a[kv] = b[kv];
      }
      
    }

    public static string Dump(Dictionary<string, string> extToMime)
    {      
      var sb = new StringBuilder();
      sb.AppendLine();
      sb.AppendFormat("Total mime types detected: {0}", extToMime.Count);
      sb.AppendLine();
      sb.AppendLine();

      foreach (var mime in extToMime.Keys.OrderBy(x => x).Select(x => new { Ext = x, Mime = extToMime[x] }))
      {
        sb.AppendFormat(
          "    <mime-mapping>\r\n        <extension>{0}</extension>\r\n        <mime-type>{1}</mime-type>\r\n    </mime-mapping>\r\n", mime.Ext, mime.Mime);

      }
      return sb.ToString();
    }


    public static Dictionary<String, String> ImportFromTomCatConfig(string file)
    {
      var doc = new XmlDocument();
      doc.Load(file);

      var man = new XmlNamespaceManager(doc.NameTable);
      man.AddNamespace("x", "http://java.sun.com/xml/ns/javaee");

      return
        doc.SelectNodes("//x:mime-mapping", man)
          .OfType<XmlElement>()
          .Select(
            x =>
            new
              {
                Ext = x.SelectSingleNode("x:extension/text()", man),
                Mime = x.SelectSingleNode("x:mime-type/text()", man)
              })
          .Where(x => x.Ext != null && x.Mime != null).Select(x => new {Ext = x.Ext.Value, Mime = x.Mime.Value})
          .Where(x => !string.IsNullOrWhiteSpace(x.Ext) && !string.IsNullOrWhiteSpace(x.Mime))
          .GroupBy(x => x.Ext, x => x.Mime)
          .ToDictionary(x => x.Key, x => x.First());
    }

    public static Dictionary<string, string> ParseApacheHTTPDMimes()
    {
      var downloadData =
        new StreamReader(
          new GZipStream(
          WebRequest.Create("http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types").GetResponse().
            GetResponseStream(),
            CompressionMode.Decompress
            )).ReadToEnd();
      
//      var downloadData = new WebClient().DownloadString();
//      Console.Out.WriteLine("s = {0}", downloadData);
      return downloadData
        .Split('\n')
        .Select(x => x.Trim())
        .Where(x => !x.StartsWith("#"))
        .Where(x => x.Length > 0)
        .Select(x => x.Split("\t ".ToCharArray()).Select(z => z.Trim()).Where(z=>z.Length > 0).ToArray())
        .Where(x=>x.Length == 2)
        .Select(x=>new {Ext = x[1].Trim(), Mime = x[0].Trim()})
        .ToDictionary(x => x.Ext, x => x.Mime);
    }
  }
}
