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
        private int myLinesToGet = 1;

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
                var takenLines = new Dictionary<int, bool>();
                var ex = new Regex(filter, RegexOptions.Compiled);
                for(var i=0; i<myContent.Count; i++)
                {
                    if (ex.Matches(myContent[i]).Count > 0)
                    {
                        for(var ln = i-myLinesToGet; ln <= i+myLinesToGet; ln++)
                        {
                            if (ln >= 0 && ln < myContent.Count)
                            {
                                bool b;
                                takenLines.TryGetValue(ln, out b);
                                takenLines[ln] = b || ln == i;
                            }
                        }
                    }
                }

                var lines = new List<int>(takenLines.Keys);
                lines.Sort();                
                
                var all = lines.ConvertAll(input => (takenLines[input] ? " " : "-") + myContent[input]);                
                ContentChanged(this, new ContentChangedEventArgs(all));
            } catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void SetLinesToGet(int lines)
        {
            myLinesToGet = lines;
        }
    }
}