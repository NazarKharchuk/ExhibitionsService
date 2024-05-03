using ExhibitionsService.BLL.Interfaces;

namespace ExhibitionsService.PL.BackgroundTasks
{
    public class ContestWinnerDeterminationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _dailyTimer;

        public ContestWinnerDeterminationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan initialDelay = CalculateInitialDelayForToday(TimeSpan.Zero /*TimeSpan.FromHours(18) + TimeSpan.FromMinutes(55)*/);
            _dailyTimer = new Timer(async _ =>
            {
                await ExecuteAsync(CancellationToken.None);
            }, null, initialDelay, TimeSpan.FromDays(1));

            Task.Run(async () =>
            {
                await ExecuteAsync(cancellationToken);
            });

            return base.StartAsync(cancellationToken);
        }

        private TimeSpan CalculateInitialDelayForToday(TimeSpan targetTime)
        {
            DateTime now = DateTime.Now;
            DateTime targetDateTime = now.Date.Add(targetTime);

            if (now > targetDateTime)
            {
                targetDateTime = targetDateTime.AddDays(1);
            }

            return targetDateTime - now;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var contestApplicationService = scope.ServiceProvider.GetRequiredService<IContestApplicationService>();

            try
            {
                await contestApplicationService.DetermineWinners();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виконання призначення переможців зіткнулось з проблемою: {ex.Message}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _dailyTimer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _dailyTimer?.Dispose();
            base.Dispose();
        }
    }
}
