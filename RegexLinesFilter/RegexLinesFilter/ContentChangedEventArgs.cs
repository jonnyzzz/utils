using System;
using System.Collections.Generic;

namespace RegexLinesFilter
{
    public class ContentChangedEventArgs : EventArgs
    {
        public readonly List<String> Lines;

        public ContentChangedEventArgs(List<string> lines)
        {
            Lines = lines;
        }
    }
}