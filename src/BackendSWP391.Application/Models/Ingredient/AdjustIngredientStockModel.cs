namespace BackendSWP391.Application.Models.Ingredient;

public class AdjustIngredientStockModel
{
    /// <summary>
    /// Số lượng cộng thêm vào tồn kho bếp trung tâm. Phải > 0.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Ghi chú/lý do (tùy chọn).
    /// </summary>
    public string? Notes { get; set; }
}

