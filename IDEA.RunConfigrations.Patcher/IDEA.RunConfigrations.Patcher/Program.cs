using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace IDEA.RunConfigrations.Patcher
{
    class Program
    {
        public static int Main(string[] argz)
        {
            if (argz.Length != 1)
            {
                Console.Out.WriteLine("Usage: <assemnly> <idea runConfigurations directory>");
                return 1;
            }

            var replacements = new Dictionary<string, string>
                                   {
                                       {"-XX:MaxPermSize=200m", "-XX:MaxPermSize=\\d+m"},
                                       {"-Xmx384m", "-Xmx\\d+m"},
                                       {"-XX:+HeapDumpOnOutOfMemoryError", "-XX:+HeapDumpOnOutOfMemoryError"},
                                       {"-ea", "-ea"},
                                   };

            foreach (var file in Directory.GetFiles(argz[0], "*.xml"))
            {
                Console.Out.WriteLine("Processing: {0}", file);
                var doc = new XmlDocument();
                doc.Load(file);

                var config = doc.SelectSingleNode("component/configuration") as XmlElement;
                if (config == null)
                {
                    Console.Out.WriteLine("Failed to find configuration name. Skip");
                    continue;
                }
                if (!config.GetAttribute("name").ToLower().Contains("[TeamCity]".ToLower()) || config.GetAttribute("type") != "TestNG")
                {
                    Console.Out.WriteLine("Wrong name of configuration: {0}", config.Value);
                    continue;
                }


                var wm = doc.SelectSingleNode("component/configuration/option[@name='VM_PARAMETERS']") as XmlElement;
                if (wm == null)
                {
                    Console.Error.WriteLine("Configuration without VM_PARAMETERS: {0}", file);
                    continue;
                }

                var text = wm.GetAttribute("value");
                Console.Out.WriteLine("Text: {0}", text);

                foreach (var e in replacements)
                {
                    var permGen = new Regex(e.Value, RegexOptions.Compiled);
                    var match = permGen.Match(text);
                    if (match.Success)
                    {
                        text = permGen.Replace(text, e.Key);
                    }
                    else
                    {
                        text += " " + e.Key;
                    }
                }
                wm.SetAttribute("value", text);
                Console.Out.WriteLine("Next: {0}", text);
                Console.Out.WriteLine("");
                Console.Out.WriteLine("");

                doc.Save(file);
            }

            return 0;
        }

    }
}
