using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace p4clientsCleaner
{
  internal class Program
  {
    private static void Main(string[] _args)
    {
      Console.Out.WriteLine("Utility to remove all perforce pending change lists without files from current workspace.");
      Console.Out.WriteLine("(C) 2011 Eugene Petrenko.");

      Console.Out.WriteLine("Usage: ");
      Console.Out.WriteLine(" p4ccc.exe [-client]");
      Console.Out.WriteLine("         -client        specifies workspace to perform the clean ");
      Console.Out.WriteLine("                        %P4CLIENT% in considered by default");
      Console.Out.WriteLine("");

      if (Environment.CommandLine.Contains("/?") || Environment.CommandLine.Contains("-?") ||
          Environment.CommandLine.Contains("-help"))
        return;

      var args = new Args(_args);

      string client = args.Get("client", Environment.GetEnvironmentVariable("P4CLIENT"));
      Console.Out.WriteLine("p4 client = {0}", client);

      var rx = new Regex("^Change\\s(\\d+)");
      try
      {
        var lists = RunCommand("p4", "changelists -s pending -c MUNIT-009")
          .Select(x =>
                    {
                      Match match = rx.Match(x);
                      if (!match.Success)
                        return (int?) null;

                      try
                      {
                        return
                          int.Parse(
                            match.Groups[1].Value);
                      }
                      catch
                      {
                        return null;
                      }
                    })
          .Where(x => x != null)
          .Select(x => x.Value)
          .ToArray();

        foreach (var list in lists)
        {
          try
          {
            RunCommand("p4", "changelist -d " + list);
          }
          catch
          {
            Console.Out.WriteLine("Failed to delete changelist {0}", list);
          }
        }
      }
      catch (Exception e)
      {
        Console.Out.WriteLine("Failed. " + e.Message);
      }
    }


    private static IEnumerable<string> RunCommand(string cmd, string argz)
    {
      var pi = new ProcessStartInfo
                 {
                   FileName = cmd,
                   Arguments = argz,
                   CreateNoWindow = true,
                   RedirectStandardError = true,
                   RedirectStandardOutput = true,
                   StandardOutputEncoding = Encoding.UTF8,
                   StandardErrorEncoding = Encoding.UTF8,
                   UseShellExecute = false
                 };

      Console.Out.WriteLine("Start command {0} {1}:", cmd, argz);
      Process process = Process.Start(pi);
      var error = new StringBuilder();
      var output = new StringBuilder();


      using (ReaderThread(process.StandardOutput, Console.Out, output))
      using (ReaderThread(process.StandardError, Console.Error, error))
      {
        process.WaitForExit();
      }

      if (process.ExitCode != 0)
      {
        throw new Exception("Failed to exeute command " + cmd + " " + argz);
      }

      return output.ToString().Split('\n');
    }

    private static IDisposable ReaderThread(StreamReader sr, TextWriter copy, StringBuilder b)
    {
      var th = new Thread(() =>
                            {
                              string i;
                              while ((i = sr.ReadLine()) != null)
                              {
                                b.AppendLine(i);
                                if (copy != null)
                                {
                                  copy.WriteLine(i);
                                }
                              }
                            });
      th.Name = "Reader thread";
      th.Start();
      return new Disposable(th.Join);
    }
  }
}