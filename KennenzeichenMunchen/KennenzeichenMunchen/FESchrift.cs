using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KennenzeichenMunchen
{
  public static class FESchrift
  {
    private static readonly PrivateFontCollection s_FontCollection = new PrivateFontCollection();

    private static FontFamily[] FontFamilies
    {
      get
      {
        if (s_FontCollection.Families.Length == 0) LoadFonts();
        return s_FontCollection.Families;
      }
    }

    public static Font Font(int sz)
    {
      return new Font(FontFamilies[0], sz);
    }

    private static void ApplyFont(string family, Control control)
    {
      foreach (FontFamily font in FontFamilies)
      {
        if (font.Name.ToLower().Equals(family.ToLower()))
        {
          control.Font = new Font(font, control.Font.Size, control.Font.Style);
        }
      }
    }

    private static void LoadFonts()
    {
      Assembly assembly = Assembly.GetEntryAssembly();
      if (assembly == null) return;

      foreach (string resource in assembly.GetManifestResourceNames())
      {
        // Load TTF files from your Fonts resource folder.
        if (!resource.ToLower().EndsWith(".ttf")) continue;

        LoadSpecialFont(resource);
      }
    }

    // Adding a private font (Win2000 and later)
    [DllImport("gdi32.dll", ExactSpelling = true)]
    private static extern IntPtr AddFontMemResourceEx(byte[] pbFont, int cbFont, IntPtr pdv, out uint pcFonts);

    // Cleanup of a private font (Win2000 and later)
    [DllImport("gdi32.dll", ExactSpelling = true)]
    internal static extern bool RemoveFontMemResourceEx(IntPtr fh);

    // Some private holders of font information we are loading
    /////////////////////////////////////
    //
    // The GetSpecialFont procedure takes a size and
    // create a font of that size using the hardcoded
    // special font name it knows about.
    //
    /////////////////////////////////////
    private static void LoadSpecialFont(string resource)
    {
      // First load the font as a memory stream
      Stream stmFont = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
      if (null == stmFont) throw new Exception("No font");

      // GDI+ wants a pointer to memory, GDI wants the memory.
      // We will make them both happy.
      //
      // First read the font into a buffer
      var rgbyt = new Byte[stmFont.Length];
      stmFont.Read(rgbyt, 0, rgbyt.Length);
      // Then do the unmanaged font (Windows 2000 and later)
      // The reason this works is that GDI+ will create a font object for
      // controls like the RichTextBox and this call will make sure that GDI
      // recognizes the font name, later.
      uint cFonts;
      AddFontMemResourceEx(rgbyt, rgbyt.Length, IntPtr.Zero, out cFonts);
      
      // Now do the managed font
      IntPtr pbyt = Marshal.AllocCoTaskMem(rgbyt.Length);
      Marshal.Copy(rgbyt, 0, pbyt, rgbyt.Length);

      s_FontCollection.AddMemoryFont(pbyt, rgbyt.Length);
      Marshal.FreeCoTaskMem(pbyt);
    }
  }
}