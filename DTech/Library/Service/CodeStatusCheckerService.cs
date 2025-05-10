using DTech.Models.EF;

namespace DTech.Library.Service
{
    public class CodeStatusCheckerService(IServiceProvider serviceProvider, ILogger<CodeStatusCheckerService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndUpdateCodeStatus();
                await CheckEndDateDiscount();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Runs every 24 hours
            }
        }

        private async Task CheckAndUpdateCodeStatus()
        {
            using (var scope = serviceProvider.CreateScope())
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
                    logger.LogInformation($"{outdatedCodes.Count} codes have been marked as outdated.");
                }
            }
        }

        private async Task CheckEndDateDiscount()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceWebContext>();
                var products = dbContext.Products
                    .Where(p => p.EndDateDiscount < DateOnly.FromDateTime(DateTime.Now) && p.Discount != 0)
                    .ToList();
                foreach (var product in products)
                {
                    product.Discount = 0;
                }
                if (products.Count > 0)
                {
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation($"{products.Count} products have been marked as outdated.");
                }
            }
        }
    }

}
