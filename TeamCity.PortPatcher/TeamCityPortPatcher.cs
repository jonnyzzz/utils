using System;
using System.IO;
using System.Xml;

namespace PatchTeamCityPort
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Out.WriteLine("TeamCity port patcher. Replaces default port 8111 to 8222");
      var home = args.Length > 0 ? args[0] : Environment.CurrentDirectory;
      home = Path.GetFullPath(home);

      Console.Out.WriteLine("Patching TeamCity ports under: " + home);
      if (!Directory.Exists(home))
      {
        Console.Out.WriteLine("Failed to patch TeamCity under: " + home);
        return;
      }

      PatchTomcatConfig(home);
      PatchAgentConfig(home);
    }

    private static void PatchTomcatConfig(string home)
    {
      var path = Path.Combine(home, "conf/server.xml");
      XmlDocument doc = new XmlDocument();
      doc.Load(path);

      doc.SelectSingleNode("Server/@port").Value = "8106";
      doc.SelectSingleNode("Server/Service/Connector/@port").Value = "8222";
      doc.SelectSingleNode("Server/Service/Connector/@redirectPort").Value = "8444";

      doc.Save(path);
    }

    public static void PatchAgentConfig(string home)
    {
      var path = Path.Combine(home, "buildAgent/conf/buildAgent.properties");

      var text = File.ReadAllText(path);
      text = text.Replace("localhost:8111", "localhost:8222");

      File.WriteAllText(path, text);
    }
  }
}
