using System.Diagnostics;

namespace Centralite.Common.Models
{
    public enum ErrorType
    {
        Error,
        Exception
    }

    public class ErrorMessage
    {
        public ErrorType ErrorType { get; set; }
        public string Message { get; set; }
        public StackTrace Trace { get; set; }

        public string DisplayMessage
        {
            get
            {
                return this.ToString();
            }
        }

        public ErrorMessage() : this(null, ErrorType.Error) { }

        public ErrorMessage(string message, ErrorType errorType = ErrorType.Error)
        {
            this.ErrorType = errorType;
            this.Message = message;
            this.Trace = new StackTrace(1);
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}", Message, (ErrorType == ErrorType.Exception) ? Trace.ToString() : string.Empty);
        }
    }
}