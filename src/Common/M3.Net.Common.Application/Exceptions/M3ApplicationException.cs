using M3.Net.Common.Domain;

namespace M3.Net.Common.Application.Exceptions;

public sealed class M3ApplicationException : Exception
{
    public M3ApplicationException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public Error? Error { get; }
}
