using Microsoft.AspNetCore.Mvc;
using PartStockManager.Adapter.Models;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [HttpPost]
        [Route("inventory")]
        public IActionResult Inventory([FromBody] InventoryRequest request)
        {
            if(request?.PartList is null || request.PartList.Count() == 0 )
                return BadRequest("The parts list to inventory can't be empty !");

            Dictionary<string, int> inventoryRequest = new Dictionary<string, int>();

            request.PartList.ForEach(p => {
                inventoryRequest.Add(p.Reference, p.Quantity);
            });
            
            var result = _stockRepository.Inventory(inventoryRequest);
            
            return result ? Ok("Inventory registered !") : NotFound("One or more references were not found in the database.");
        }

        [HttpPost]
        [Route("exit")]
        public IActionResult RecordStockExit([FromBody] StockMovementRequest request)
        {
            try
            {
                var result = _stockRepository.RecordStockExit(request.Reference, request.Quantity);
                return result ? Ok($"Stock exit done (Part Reference: {request.Reference}, Quantity added: {request.Quantity})") : NotFound($"Part not found (Part Reference : {request.Reference})");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("entry")]
        public IActionResult RecordStockEntry([FromBody] StockMovementRequest request)
        {
            try
            {
                var result = _stockRepository.RecordStockEntry(request.Reference, request.Quantity);
                return result ? Ok($"Stock entry done (Part Reference: {request.Reference}, Quantity added: {request.Quantity})") : NotFound($"Part not found (Part Reference : {request.Reference})");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
