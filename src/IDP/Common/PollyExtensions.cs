using Polly;

namespace IDP.Common;

public static class PollyExtensions
{
    public static IServiceCollection RegisterResiliencePipeline(this IServiceCollection services)
    {
        return
        services.AddResiliencePipeline(CommonConstants.ResiliencePipeline, builder =>
        {
            builder.AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                Delay = TimeSpan.FromMicroseconds(300),
                MaxDelay = TimeSpan.FromMilliseconds(10000),
                MaxRetryAttempts = 10,
                ShouldHandle = new PredicateBuilder().Handle<Exception>()
            });
        });
    }

}
