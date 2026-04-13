namespace PartStockManager.Adapter.Models
{
    public class InventoryRequest
    {
        public InventoryRequest(List<InventoryItem> partList)
        { 
            PartList = partList;
        }
        public List<InventoryItem> PartList { get; set; }
    }
}
