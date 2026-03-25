#nullable disable
using System;

namespace BackendSWP391.Core.Models;

public class StoreCostRecord
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public decimal Cost { get; set; }

    /// <summary>'WasteCost' hoặc 'OperatingCost'</summary>
    public string CostType { get; set; }

    public DateTime OccurredAt { get; set; }

    public string Notes { get; set; }

    public virtual FranchiseStore Store { get; set; }

    public virtual Ingredient Ingredient { get; set; }
}
