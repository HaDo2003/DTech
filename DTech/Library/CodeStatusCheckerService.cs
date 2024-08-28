using DTech.Models.EF;

namespace DTech.Library
{
    public class CodeStatusCheckerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CodeStatusCheckerService> _logger;

        public CodeStatusCheckerService(IServiceProvider serviceProvider, ILogger<CodeStatusCheckerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndUpdateCodeStatus();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Runs every hour
            }
        }

        private async Task CheckAndUpdateCodeStatus()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceWebContext>();

                var outdatedCodes = dbContext.Coupons
                    .Where(c => c.EndDate < DateOnly.FromDateTime(DateTime.Now) && c.Status != 2)
                    .ToList();

                foreach (var code in outdatedCodes)
                {
                    code.Status = 2;
                }

                if (outdatedCodes.Count > 0)
                {
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"{outdatedCodes.Count} codes have been marked as outdated.");
                }
            }
        }
    }

}
