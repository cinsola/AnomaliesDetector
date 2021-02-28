using System;

namespace Common.Exceptions
{
    public class InvalidLogException : Exception
    {
        public InvalidLogException(string message) : base(message)
        {
        }

        public InvalidLogException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
