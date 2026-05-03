using PartStockManager.CoreLogic.Models;

namespace PartStockManager.CoreLogic.Repositories
{
    public interface IStockRepository
    {
        bool Inventory(Dictionary<string, int> partsList);

        bool RecordStockExit(string reference, int quantityToRemove);
        bool RecordStockEntry(string reference, int quantityToAdd);
    }
}
