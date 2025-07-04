namespace M3.Net.Common.Application.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
