#nullable disable
using System;

namespace BackendSWP391.Core.Models;

public class StoreIngredientStock
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public int IngredientId { get; set; }

    public decimal CurrentStock { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual FranchiseStore Store { get; set; }

    public virtual Ingredient Ingredient { get; set; }
}
