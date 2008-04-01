using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            
            myModel.ContentChanged += OnContentChanged;
            myModel.DocumentUpdated += delegate
                                           {
                                               myFilter_Click(this, null);
                                           };            
        }

        private void OnContentChanged(object _, ContentChangedEventArgs e)
        {            
            myDoc.Text = string.Join(Environment.NewLine, e.Lines.ToArray());
        }

        private void myFilter_Click(object sender, RoutedEventArgs e)
        {
            myModel.SetFilter(myRegex.Text.Trim());
        }

        private void myFileOpenMenu_Click_utf16le(object sender, RoutedEventArgs e)
        {
            DoOpen(Encoding.Unicode);
        }

        private void DoOpen(Encoding unicode)
        {
            var dlg = new OpenFileDialog();
            if (true == dlg.ShowDialog(this))
            {                
                myModel.LoadFile(dlg.FileName, unicode);
            }
        }

        private void myRegex_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                myFilter_Click(sender, e);
            }
        }

        private void myFileOpenMenu_Click_utf8(object sender, RoutedEventArgs e)
        {
            DoOpen(Encoding.UTF8);
        }

        private void myLines_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lines;
            if (int.TryParse(myLines.Text, out lines))
                myModel.SetLinesToGet(lines);
        }
    }
}
