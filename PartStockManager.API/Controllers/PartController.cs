using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PartStockManager.Adapter.Models;
using PartStockManager.Adapter.Repositories;
using PartStockManager.CoreLogic.Models;
using PartStockManager.CoreLogic.Repositories;
using Serilog.Core;

namespace PartStockManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartController : ControllerBase
    {
        private readonly IPartRepository _partRepository;
        private readonly ILogger<PartController> _logger;

        public PartController(IPartRepository partRepository, ILogger<PartController> logger)
        {
            _partRepository = partRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreatePart([FromBody] PartDto request)
        {
            _logger.LogInformation($"API Request: CreatePart for reference {request.Reference}");

            if (string.IsNullOrEmpty(request.Reference))
            {
                _logger.LogWarning("CreatePart failed: the reference is missing !");
                return BadRequest("Reference is required.");
            }

            try
            {
                var newPart = new Part(
                    request.Name,
                    request.Reference,
                    request.StockQuantity,
                    request.LowStockThreshold
                );

                var result = _partRepository.CreatePart(newPart);

                return result ? Ok($"Part successfully created (Reference: '{newPart.Reference}')") : Conflict($"A part with the reference '{request.Reference}' already exists !");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API Request - CreatePart: An error occurred during CreatePart for {request.Reference}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Route("modify")]
        public IActionResult ModifyPart([FromBody] PartModificationRequest request)
        {
            _logger.LogInformation($"API Request: ModifyPart for reference {request.CurrentPartReference}");

            PartDto updatedPart = request.UpdatedPart;

            try
            {
                string partReference = request.CurrentPartReference;

                var newPart = new Part(
                    request.UpdatedPart.Name,
                    request.UpdatedPart.Reference,
                    request.UpdatedPart.StockQuantity,
                    request.UpdatedPart.LowStockThreshold
                );

                var result = _partRepository.ModifyPart(partReference, newPart);

                return result ? Ok($"Part successfully modified (Old Reference: {request.CurrentPartReference}, Current Reference: '{newPart.Reference}')") : NotFound($"Error during part update (Reference: '{request.CurrentPartReference}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API Request - ModifyPart: An error occurred during ModifyPart for {request.CurrentPartReference}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult DeletePart([FromBody] PartDeletionRequest request)
        {
            _logger.LogInformation($"API Request: DeletePart for reference {request.PartReference}");

            try
            {
                var result = _partRepository.DeletePart(request.PartReference);
                return result ? Ok($"Part successfully deleted (Reference: '{request.PartReference}')") : NotFound($"No part with the reference '{request.PartReference}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API Request - DeletePart: An error occurred during DeletePart for {request.PartReference}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("get/parts")]
        public IActionResult GetParts([FromQuery] string name = "", [FromQuery] string reference = "")
        {
            _logger.LogInformation($"API Request: GetParts, Name: '{name}', Reference '{reference}'");

            try
            {
                var result = _partRepository.GetParts(name, reference);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API Request - GetParts: An error occurred when searching parts (Name :'{name}', Reference: '{reference}')");
                return StatusCode(500, "Internal server error while retrieving parts.");
            }
        }

        [HttpGet]
        [Route("get/reached-threshold")]
        public IActionResult GetPartsWithReachedThreshold()
        {
            _logger.LogInformation($"API Request: GetPartsWithReachedThreshold");

            try
            {
                var parts = _partRepository.GetPartsWithReachedThreshold();

                return Ok(parts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API Request - GetPartsWithReachedThreshold: An error occurred when searching parts with reached threshold");
                return StatusCode(500, "Internal server error while retrieving parts.");
            }
        }
    }
}
