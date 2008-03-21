using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace RegexLinesFilter
{
    public class Model
    {
        private readonly List<String> myContent = new List<string>();

        public EventHandler<ContentChangedEventArgs> ContentChanged;
        public EventHandler<DocumentUpdatedEventArgs> DocumentUpdated;

        
        public void LoadFile(string file, Encoding encoding)
        {
            myContent.Clear();
            using(Stream str = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(TextReader tr = new StreamReader(str, encoding))
                {
                    
                    while(true)
                    {
                        var s = tr.ReadLine();
                        if (s == null)
                            break;
                        myContent.Add(s);
                    }
                }
            }
            DocumentUpdated(this, new DocumentUpdatedEventArgs(file));
        }

        public void SetFilter(string filter)
        {
            try
            {
                var ex = new Regex(filter, RegexOptions.Compiled);
                var all = myContent.FindAll(x => ex.Matches(x).Count > 0);
                
                ContentChanged(this, new ContentChangedEventArgs(all));
            } catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}