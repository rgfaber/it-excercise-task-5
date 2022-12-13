using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestKit;

public static partial class Inject
{
    public static IServiceCollection AddBaseTestEnv(this IServiceCollection services)
    {
        return services
            .AddHostExecutor()
            .AddTransient<ITestOutputHelper, TestOutputHelper>()
            .AddSingleton<ITestHelper, TestHelper>();
    }
   
}