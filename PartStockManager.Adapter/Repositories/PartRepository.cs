using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using PartStockManager.Adapter.Database.Context;
using PartStockManager.Adapter.Database.Entities;
using PartStockManager.CoreLogic.Models;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.Adapter.Repositories
{
    public class PartRepository : IPartRepository
    {
        private readonly StockDbContext _dbContext;
        private readonly ILogger<PartRepository> _logger;

        public PartRepository(StockDbContext dbContext, ILogger<PartRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public bool CreatePart(Part newPart)
        {
            _logger.LogInformation($"CreatePart: Creation of the part '{newPart.Reference}' !");

            if (_dbContext.Parts.Any(p => p.Reference == newPart.Reference))
            {
                _logger.LogError($"CreatePart: Failed, Reference '{newPart.Reference}' already exists !");
                return false;
            }

            try
            {
                var newEntity = new PartEntity()
                {
                    Name = newPart.Name,
                    Reference = newPart.Reference,
                    StockQuantity = newPart.StockQuantity,
                    LowStockThreshold = newPart.LowStockThreshold,
                };

                _dbContext.Parts.Add(newEntity);

                var result = _dbContext.SaveChanges();                

                if(result > 0)
                {
                    _logger.LogInformation($"CreatePart: Success, part '{newPart.Reference}' created !");
                    return true;
                }
                else
                {
                    _logger.LogError($"CreatePart: Failed, part '{newPart.Reference}' not created !");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CreatePart: Failed, error during the creation of the part '{newPart.Reference}' !");
                throw;
            }
		}

		public bool ModifyPart(string currentRef, Part modifiedPart)
        {
            _logger.LogInformation($"ModifyPart: Modification of the part '{currentRef}' !");

            try
			{
				var entity = _dbContext.Parts.FirstOrDefault(p => p.Reference == currentRef);

				if (entity == null)
					return false;

				entity.Name = modifiedPart.Name;
				entity.Reference = modifiedPart.Reference;
				entity.StockQuantity = modifiedPart.StockQuantity;
				entity.LowStockThreshold = modifiedPart.LowStockThreshold;

                var result = _dbContext.SaveChanges();

                if (result > 0)
                {
                    _logger.LogInformation($"ModifyPart: Success, part '{currentRef}' modified !");
                    return true;
                }
                else
                {
                    _logger.LogError($"ModifyPart: Failed, part '{currentRef}' not modified !");
                    return false;
                }
            }
			catch (Exception ex)
			{
                _logger.LogError(ex, $"ModifyPart: Failed, error during the modification of the part '{currentRef}' !");
                throw;
			}
		}

		public bool DeletePart(string reference)
        {
            _logger.LogInformation($"DeletePart: Deletion of the part '{reference}' !");

            try
            {
                var entityToRemove = _dbContext.Parts.FirstOrDefault(p => p.Reference == reference);

                if (entityToRemove == null)
                {
                    _logger.LogError($"DeletePart: Part '{reference}' doesn't exist !");
                    return false;
                }

                _dbContext.Parts.Remove(entityToRemove);

                var result = _dbContext.SaveChanges();

                if (result > 0)
                {
                    _logger.LogInformation($"DeletePart: Success, part '{reference}' deleted !");
                    return true;
                }
                else
                {
                    _logger.LogError($"DeletePart: Failed, part '{reference}' not deleted !");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeletePart: Error during the deletion of the part '{reference}'");
                return false;
            }
        }

        public List<Part> GetParts(string name, string reference)
        {
            _logger.LogInformation($"GetParts: Parts research launched !");

            try
            {
                List<Part> result = _dbContext.Parts.Where(p => p.Name.ToLower().Contains(name.ToLower()) && p.Reference.ToLower().Contains(reference.ToLower()))
                    .Select(p => new Part(p.Name, p.Reference, p.StockQuantity, p.LowStockThreshold)).ToList();

                _logger.LogInformation($"GetParts: Success, the parts have been retrieved !");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetParts: Error when searching parts !");
                throw;
            }
        }

        public List<Part> GetPartsWithReachedThreshold()
        {
            _logger.LogInformation($"GetPartsWithReachedThreshold: Get parts with reached threshold !");

            try
            {
                List<Part> result = _dbContext.Parts.Where(p => p.StockQuantity <= p.LowStockThreshold)
                    .Select(p => new Part(p.Name, p.Reference, p.StockQuantity, p.LowStockThreshold)).ToList();

                _logger.LogInformation($"GetPartsWithReachedThreshold: Success, the parts with reached threshold have been retrieved !");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetPartsWithReachedThreshold: Error when getting parts with reached threshold !");
                throw;
            }
        }
        
    }
}
