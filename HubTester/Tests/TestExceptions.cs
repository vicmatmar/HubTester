using System;

namespace HubTester.Tests
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
