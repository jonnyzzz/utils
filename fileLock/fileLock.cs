using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public static class FileLock {
  public static int Main(string[] argz) {
    Console.Out.WriteLine("FileLock utility. (C) Eugene Petrenko 2011");
    Console.Out.WriteLine();
   
    Dictionary<string, Stream> locks = new Dictionary<string, Stream>();
    foreach(string arg in argz) {
      if (!File.Exists(arg)) {
        Console.Out.WriteLine("Error: File {0} does not exist!", arg);
        continue;
      }

      locks[arg] = new FileStream(arg, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }

    Console.In.Read();

    foreach(Stream s in locks.Values) {
      s.Close();
    }
    return 0;
  }

  private static int Usage() {
    Console.Out.WriteLine("Usage: ");
    Console.Out.WriteLine("  FileLock.exe fileToLock [fileToLock2 ... ]");
    Console.Out.WriteLine("");
    return 1;
  }
}
