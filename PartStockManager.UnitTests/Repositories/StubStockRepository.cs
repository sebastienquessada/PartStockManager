using PartStockManager.CoreLogic.Models;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.Tests.Repositories
{
    public class StubStockRepository : IStockRepository
    {
        public List<Part> StubPartsList { get; set; }
        public Part StubPart { get; set; }
       
        public bool Inventory(Dictionary<string, int> partsList)
        {
            StubPartsList.ForEach(p =>
            {
                int newQuantity = partsList.Where(part => part.Key == p.Reference).First().Value;
                p.AdjustStock(newQuantity);
            });

            return true;
        }

        public bool RecordStockExit(string reference, int quantityToRemove)
        {
            var part = StubPartsList.Where(p => p.Reference == reference).FirstOrDefault();

            if (part == null)
                return false;

            if(quantityToRemove < 0 || quantityToRemove > part.StockQuantity)
                return false;

            int newQuantity = part.StockQuantity - quantityToRemove;

            part.AdjustStock(newQuantity);

            return true;
        }
        public bool RecordStockEntry(string reference, int quantityToAdd)
        {
            var part = StubPartsList.Where(p => p.Reference == reference).FirstOrDefault();

            if (part == null)
                return false;

            if (quantityToAdd < 0)
                return false;

            int newQuantity = part.StockQuantity + quantityToAdd;

            part.AdjustStock(newQuantity);

            return true;
        }
    }
}
