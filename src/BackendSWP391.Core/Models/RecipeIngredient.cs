#nullable disable
using System;
using System.Collections.Generic;

namespace BackendSWP391.Core.Models;

public partial class RecipeIngredient
{
    public int RecipeIngredientId { get; set; }

    public int RecipeId { get; set; }

    public int IngredientId { get; set; }

    public decimal? Quantity { get; set; }

    public virtual Recipe Recipe { get; set; }

    public virtual Ingredient Ingredient { get; set; }
}
