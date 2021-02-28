using System;

namespace Common.Exceptions
{
    public class InvalidLogRowException : Exception
    {
        public InvalidLogRowException(string message) : base(message)
        {
        }

        public InvalidLogRowException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
