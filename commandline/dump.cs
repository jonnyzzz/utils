public class Dump {
  public static void Main(string[] args) {
    foreach(string s in args) {
       System.Console.Out.WriteLine("Arg: {0}", s);
    }
  }
}