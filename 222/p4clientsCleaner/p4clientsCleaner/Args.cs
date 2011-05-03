using System;
using System.Collections.Generic;
using System.Linq;

namespace p4clientsCleaner
{
  public class Args
  {
    private readonly List<string> myArgs;

    public Args(IEnumerable<string> args)
    {
      myArgs = new List<string>(args);
    }

    public string this[int i]
    {
      get
      {
        try
        {
          return myArgs[i];
        }
        catch
        {
          return null;
        }
      }
    }

    public bool Contains(string key)
    {
      return myArgs.Any(arg => arg.Equals("-" + key, StringComparison.InvariantCultureIgnoreCase));
    }

    public string Get(string key, string def)
    {
      string lookup = "-" + key + "=";
      foreach (string arg in myArgs)
      {
        if (arg.StartsWith(lookup, StringComparison.InvariantCultureIgnoreCase))
        {
          return arg.Substring(lookup.Length);
        }
      }
      return def;
    }
  }
}