namespace MyPosTask.Infrastructure.Data.Interceptors;

public class SystemTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}
