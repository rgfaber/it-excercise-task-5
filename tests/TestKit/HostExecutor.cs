using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestKit;

public static partial class Inject
{
    public static IServiceCollection AddHostExecutor(this IServiceCollection services)
    {
        return services?
            .AddTransient<IHostExecutor, HostExecutor>();
    }
}

public interface IHostExecutor
{
    Task StartAsync(CancellationToken token = default);
    Task StopAsync(CancellationToken token = default);
}

public class HostExecutor : IHostExecutor
{
    private readonly IEnumerable<IHostedService> _services;

    public HostExecutor(IEnumerable<IHostedService> services)
    {
        _services = services;
    }

    public Task StartAsync(CancellationToken token)
    {
        return ExecuteAsync(service => service.StartAsync(token));
    }

    public Task StopAsync(CancellationToken token)
    {
        return ExecuteAsync(service => service.StopAsync(token));
    }

    private Task ExecuteAsync(Func<IHostedService, Task> callback)
    {
        List<Exception> exceptions = null;

        foreach (var service in _services)
            try
            {
                callback(service);
            }
            catch (Exception ex)
            {
                exceptions ??= new List<Exception>();

                exceptions.Add(ex);
            }

        // Throw an aggregate exception if there were any exceptions
        if (exceptions != null) throw new AggregateException(exceptions);
        return Task.CompletedTask;
    }
}

