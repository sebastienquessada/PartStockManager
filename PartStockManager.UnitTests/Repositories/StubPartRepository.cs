using PartStockManager.CoreLogic.Models;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.Tests.Repositories
{
    public class StubPartRepository : IPartRepository
    {
        public List<Part> StubPartsList { get; set; }
        public Part StubPart { get; set; }
        public bool CreatePart(Part newPart)
        {
            if(StubPartsList.Any(p => p.Reference == newPart.Reference))
                return false;

            StubPart = new Part(newPart.Name, newPart.Reference, newPart.StockQuantity, newPart.LowStockThreshold);

            return true;
        }

        public bool ModifyPart(string currentRef, Part modifiedPart)
        {
            StubPartsList.Where(p => p.Reference == currentRef).First().UpdateInfo(modifiedPart.Name, modifiedPart.Reference, modifiedPart.LowStockThreshold);

            return true;
        }

        public bool DeletePart(string reference)
        {
            if (StubPartsList.Where(p => p.Reference == reference).FirstOrDefault() is null)
                return false;

            StubPartsList.RemoveAll(p => p.Reference == reference);

            return true;
        }

        public List<Part> GetParts(string name, string reference)
        {
            return StubPartsList.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && p.Reference.Contains(reference, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Part> GetPartsWithReachedThreshold()
        {
            return StubPartsList.Where(p => p.StockQuantity <= p.LowStockThreshold).ToList();
        }
    }
}
