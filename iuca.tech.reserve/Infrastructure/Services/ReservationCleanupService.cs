using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ReservationCleanupService : IHostedService, IDisposable
{
    private readonly ILogger<ReservationCleanupService> _logger;
    private readonly Timer _timer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReservationCleanupService(ILogger<ReservationCleanupService> logger,
                                      IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _timer = new Timer(async _ => await CheckReservations(null), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    private async Task CheckReservations(object state)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var equipmentRequestService = scope.ServiceProvider.GetRequiredService<IEquipmentRequestService>();
            await equipmentRequestService.CancelExpiredRequests();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => CheckReservations(null), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
