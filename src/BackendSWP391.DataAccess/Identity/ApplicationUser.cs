using Microsoft.AspNetCore.Identity;

namespace BackendSWP391.DataAccess.Identity;

public class ApplicationUser : IdentityUser
{
    public string? Address { get; set; }

    /// <summary>FK đến FranchiseStore — chỉ set cho Franchise Store Staff</summary>
    public int? StoreId { get; set; }

    /// <summary>FK đến CentralKitchen — chỉ set cho Central Kitchen Staff</summary>
    public int? CentralKitchenId { get; set; }
}

