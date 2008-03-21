using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace RegexLinesFilter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        private readonly Model myModel = new Model();

        public Window1()
        {
            InitializeComponent();
            
            myModel.ContentChanged += 
                delegate(object _, ContentChangedEventArgs e)                
                                          {
                                              myDoc.Text = string.Join(Environment.NewLine, e.Lines.ToArray());
                                          };
            myModel.DocumentUpdated += delegate
                                           {
                                               myFilter_Click(this, null);
                                           };            
        }

        private void myFilter_Click(object sender, RoutedEventArgs e)
        {
            myModel.SetFilter(myRegex.Text.Trim());
        }

        private void myFileOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (true == dlg.ShowDialog(this))
            {
                myModel.LoadFile(dlg.FileName, Encoding.Unicode);
            }
        }

        private void myRegex_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                myFilter_Click(sender, e);
            }
        }        
    }
}
