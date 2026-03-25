using BackendSWP391.Application.Models.StoreInventory;

namespace BackendSWP391.Application.Services;

public interface IStoreInventoryService
{
    /// <summary>Lấy danh sách tồn kho nguyên liệu của một Franchise Store.</summary>
    Task<List<StoreStockDto>> GetStoreStockAsync(int storeId);

    /// <summary>
    /// Ghi nhận bán hàng tại cửa hàng: dựa trên Recipe của Product để trừ nguyên liệu
    /// và tạo bản ghi OperatingCost tương ứng.
    /// </summary>
    Task SellAtStoreAsync(int storeId, int productId, int quantity);

    /// <summary>
    /// Kiểm tra và xử lý các lô nguyên liệu đã hết hạn: đặt tồn kho về 0
    /// và tạo bản ghi WasteCost.
    /// </summary>
    Task ProcessExpiredItemsAsync(int storeId);

    /// <summary>
    /// Cập nhật kho cửa hàng khi xác nhận nhận hàng từ CK:
    /// phân tích ShipmentLines → Recipe → Ingredients rồi cộng vào StoreIngredientStock.
    /// </summary>
    Task AddStockFromShipmentAsync(int shipmentId, int storeId);

    /// <summary>Lấy danh sách bản ghi chi phí (WasteCost / OperatingCost) của cửa hàng.</summary>
    Task<List<StoreCostRecordDto>> GetCostRecordsAsync(int storeId, string? costType);
}
