using System;
using System.IO;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.PE;

namespace JetBrains.TeamCity.Utils.PE
{
  public class PEReader
  {
    public static string DescribeAssemblyRuntime(string assemblyFile)
    {
      using (Stream stream = File.OpenRead(assemblyFile))
      {
        try
        {
          var reader = new ImageReader(stream);
          reader.ReadImage();
          Image img = reader.Image;

          var sb = new StringBuilder();
          sb.Append(img.Runtime);
          sb.Append(", ");
          if (img.Architecture == TargetArchitecture.AMD64 || img.Architecture == TargetArchitecture.IA64)
          {
            sb.Append("x64");
          }
          else if ((img.Attributes & ModuleAttributes.Required32Bit) != 0)
          {
            sb.Append("x86");
          }
          else
          {
            sb.Append("MSIL");
          }

          return sb.ToString();
        }
        catch (Exception e)
        {
          return "???";
        }
      }
    }
  }
}