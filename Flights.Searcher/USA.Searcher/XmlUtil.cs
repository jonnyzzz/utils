using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace USA.Searcher
{
  public static class XmlUtil
  {
    public static string ToXml<T>(this T t)
    {
      var ww = new StringWriter();
      Write(t, ww);
      return ww.ToString();
    }

    private static void Write<T>(T t, TextWriter ww)
    {
      var sw = new XmlTextWriter(ww)
      {
        IndentChar = ' ',
        Indentation = 2,
        Formatting = Formatting.Indented
      };
      new XmlSerializerFactory().CreateSerializer(typeof (T)).Serialize(sw, t);
      sw.Close();
    }

    public static void ToXml<T>(this T t, string path)
    {
      using (var file = File.CreateText(path))
      {
        Write(t, file);
      }
    }

    public static T FromXML<T>(this string path)
    {
      using (var file = File.OpenText(path))
      {
        return (T) new XmlSerializerFactory().CreateSerializer(typeof (T)).Deserialize(file);
      }
    }
  }
}