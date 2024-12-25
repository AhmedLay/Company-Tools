
namespace CTBX.CommonUtils;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
