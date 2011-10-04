using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace gen
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Out.WriteLine("Generated dependent project.");
      Console.Out.WriteLine(" <app> <teamcity proj .xml file> <number of builds> ");

      string projectId = args.ElementAtOrDefault(0) ?? "jp_1";
      int totalNodes = int.Parse(args.ElementAtOrDefault(1) ?? "20");
      string mode = args.ElementAtOrDefault(2) ?? "full";

      Func<int, string> bt = x => projectId + "_" + (totalNodes + 5 - x).ToString("0000");

      
      string docText;
      switch (mode)
      {
        case "full":
          docText = generateProjectHeader(projectId, Range(totalNodes).Select(i => renderBuildType(bt(i), Range(i - 1).Select(bt), Range(i - 1).Select(bt))));
          break;
        case "linear":
          {
            Func<int, IEnumerable<string>> deps = i => i <= 1 ? new string[0] : new[] {bt(i - 1)};
            docText = generateProjectHeader(projectId,
                                            Range(totalNodes).Select(i => renderBuildType(bt(i), deps(i), deps(i))));
            break;
          }
        case "linear-snapshot":
          {
            Func<int, IEnumerable<string>> deps = i => i <= 1 ? new string[0] : new[] {bt(i - 1)};
            docText = generateProjectHeader(projectId, Range(totalNodes).Select(i => renderBuildType(bt(i), deps(i), new String[0])));
            break;
          }
        case "star-snapshot":
          {            
            string btRoot = bt(totalNodes+1);
            docText = generateProjectHeader(projectId, Range(totalNodes).Select(i => renderBuildType(bt(i), g(btRoot), new String[0])).Union(g(renderBuildType(btRoot, new String[0], new String[0]))));
            break;
          }
        default:
          docText = "";
          break;
      }

      var doc = new XmlDocument();
      doc.XmlResolver = null;
      doc.LoadXml(docText);      
      using (TextWriter tw = File.CreateText("project-config.xml"))doc.Save(tw);

    }



    private static IEnumerable<T>  GZm<T>(int i, Func<T> f)
    {
      if (i > 1)
        yield return f();
    }

    private static T If<T>(bool b, Func<T> then, Func<T> els)
    {
      if (b) return then();
      return els();
    }

    private static IEnumerable<T> g<T>(T t)
    {
      yield return t;
    }

    private static IEnumerable<int> Range(int z)
    {
      for (int i = 0; i < z; i++) yield return i;
    } 

    private static string generateProjectHeader(string projectId, IEnumerable<string> children )
    {      
      return @"
<!DOCTYPE project SYSTEM ""../project-config.dtd"">
<project id=""" + projectId + @""">

" +
             string.Join("\r\n", children) + @"
</project>";
    }

    private static string IFc<T>(IEnumerable<T> items, string pre, Func<T, string> present, string post)
    {
      items = items.ToArray();
      if (items.Count() > 0)
      {
        return string.Join("\r\n", g(pre).Union(items.Select(present).Union(g(post))));
      }

      return "";
    }
      
    private static String renderBuildType(string btId, IEnumerable<string> snapshotDepIds, IEnumerable<string> artifactDedIds )
    {
      return
        @"  <build-type id=""" + btId + @""" name=""" + btId +
        @""">
    <description />
    <settings>
      <parameters />
      <build-runners />
      <vcs-settings />
      <requirements />
      <build-triggers/>
      <artifact-publishing paths=""e:\a.bat"" />"
        + IFc(artifactDedIds, "<artifact-dependencies>",
              depId =>
              @"        <dependency sourceBuildTypeId=""" + depId +
              @""" cleanDestination=""false"">
               <revisionRule name=""sameChainOrLastFinished"" revision=""latest.sameChainOrLastFinished"" />
          <artifact sourcePath=""**/*"" />
        </dependency> 
",
              @" </artifact-dependencies> ") +

        IFc(snapshotDepIds, @"
      <dependencies>
",
            depId =>
            @"     <depend-on sourceBuildTypeId=""" + depId + @""">
        </depend-on> 
",
            "</dependencies>") +
        @"
      
      <cleanup />
    </settings>
  </build-type>
";
    }
  }
}
