using PartStockManager.CoreLogic.Models;

namespace PartStockManager.CoreLogic.Repositories
{
    public interface IPartRepository
    {
        bool CreatePart(Part newPart);
        bool ModifyPart(string currentRef, Part modifiedPart);
        bool DeletePart(string reference);

        List<Part> GetParts(string name, string reference);
        List<Part> GetPartsWithReachedThreshold();
    }
}
