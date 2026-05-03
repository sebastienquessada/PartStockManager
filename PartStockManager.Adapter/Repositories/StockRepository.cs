using Microsoft.Extensions.Logging;
using PartStockManager.Adapter.Database.Context;
using PartStockManager.Adapter.Database.Entities;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.Adapter.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockDbContext _dbContext;
        private readonly ILogger<StockRepository> _logger;

        public StockRepository(StockDbContext dbContext, ILogger<StockRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public bool Inventory(Dictionary<string, int> partsList)
        {
            _logger.LogInformation($"Attempting to make an inventory.");

            if (partsList is null || partsList.Count == 0)
            {
                _logger.LogError($"The inventory failed: empty part list !");
                throw new ArgumentException("The part list can't be empty !");
            }

            var references = partsList.Keys.ToList();
            var entities = _dbContext.Parts.Where(p => references.Contains(p.Reference)).ToList();

            if (entities.Count != partsList.Count)
            {
                _logger.LogError($"The inventory failed: some parts don't exist !");
                return false;
            }

            foreach (var item in partsList)
            {
                var partToUpdate = entities.FirstOrDefault(p => p.Reference == item.Key);

                if (partToUpdate != null)
                    partToUpdate.StockQuantity = item.Value;
            }

            try
            {
                _dbContext.SaveChanges();
                _logger.LogInformation($"Inventory done !");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save the inventory !");
                throw;
            }
        }

        public bool RecordStockExit(string reference, int quantityToRemove)
        {
            _logger.LogInformation($"Attempting stock exit for {reference}, quantity to remove: {quantityToRemove}");

            PartEntity part = _dbContext.Parts.FirstOrDefault(p => p.Reference == reference);

            if (part == null)
            {
                _logger.LogError($"Failed to record stock exit for '{reference}': part not found !");
                return false;
            }

            if (quantityToRemove > part.StockQuantity)
            {
                _logger.LogError($"Failed to record stock exit for '{reference}': insufficient quantity (Quantity to remove from stock: {quantityToRemove}, Quantity in stock: {part.StockQuantity})");
                throw new ArgumentOutOfRangeException($"Insufficient quantity !");
            }

            part.StockQuantity -= quantityToRemove;

            try
            {
                var result = _dbContext.SaveChanges();

                _logger.LogInformation($"Stock exit done : {quantityToRemove} parts of '{reference}' removed from the stock !");

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to record stock exit for '{reference}' !");
                throw;
            }
        }

        public bool RecordStockEntry(string reference, int quantityToAdd)
        {
            _logger.LogInformation($"Attempting stock entry for {reference}, quantity to add: {quantityToAdd}");

            PartEntity part = _dbContext.Parts.FirstOrDefault(p => p.Reference == reference);

            if (part == null)
            {
                _logger.LogError($"Failed to record stock entry for '{reference}': part not found !");
                return false;
            }

            part.StockQuantity += quantityToAdd;

            try
            {
                var result = _dbContext.SaveChanges();

                _logger.LogInformation($"Stock entry done : {quantityToAdd} parts of '{reference}' added to the stock !");

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to record stock entry for '{reference}' !");
                throw;
            }
        }
    }
}
