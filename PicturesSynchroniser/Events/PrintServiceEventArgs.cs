using System;

namespace PicturesSynchroniser.Events
{
    public class PrintServiceEventArgs : EventArgs
    {
        public PrintServiceEventArgs()
        { 
        }

        public PrintServiceEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; set; }
    }
}
