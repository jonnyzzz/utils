using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mshtml;

namespace LingvoOnline
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private string SearchText
    {
      get { return mySearch.Text.Trim(); }
      set { mySearch.Text = (value ?? "").Trim(); }
    }

    private void UpdateSearch()
    {
      myWeb.Navigate("http://lingvopro.abbyyonline.com/ru/Translate/de-ru/" + SearchText);
    }

    private void Form1_Activated(object sender, EventArgs e)
    {
      var text = (Clipboard.GetText() ?? "").Trim();
      text = Regex.Replace(text, "[\\.,-:;\\s]+", "");
      if (!string.IsNullOrWhiteSpace(text) && !text.Equals(SearchText))
      {
        SearchText = text;
        UpdateSearch();
      }
    }

    private void mySearch_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        UpdateSearch();
      }
    }

    private void myWeb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      dynamic doc = myWeb.Document.DomDocument;
      if (doc == null) return;

      foreach (string clazz in new[] { "footermenu", "b-comments-facebook", "b-user-links", "b-glossary", "b-head__r-col", "home", "mainmenu", "topmenu", "b-right-panel", "js-correct-height", "b-useful-info-after-searchform" })
      {
        foreach (dynamic o in doc.getElementsByClassName(clazz))
        {
          o.parentNode.removeChild(o);
        }
      }

    }
  }
}
