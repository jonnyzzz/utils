using System.Xml;
using System;
using System.IO;

public class Program {

  public static int Main(string[] args) {
    string input = args[0];
    string output = args.Length > 1 ? args[1] : args[0] + ".format";
  
    XmlDocument doc = new XmlDocument(); 
    doc.Load(input);
    using(TextWriter tw = File.CreateText(output))
      doc.Save(tw);

    return 0;
  }
}