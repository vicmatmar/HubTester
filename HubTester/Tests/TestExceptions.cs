using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubTest
{
    public class ReadUntilTimeoutException : Exception
    {
        public ReadUntilTimeoutException(string message) : base(message)
        {
        }

    }
    public class WriteCommandException : Exception
    {
        public WriteCommandException(string message) : base(message)
        {
        }
    }
}
