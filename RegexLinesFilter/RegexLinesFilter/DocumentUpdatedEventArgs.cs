using System;

namespace RegexLinesFilter
{
    public class DocumentUpdatedEventArgs : EventArgs
    {
        public readonly string File;

        public DocumentUpdatedEventArgs(string file)
        {
            File = file;
        }
    }
}