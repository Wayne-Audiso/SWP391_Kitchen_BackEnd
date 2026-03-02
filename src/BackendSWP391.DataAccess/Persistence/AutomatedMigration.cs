using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Identity;

namespace BackendSWP391.DataAccess.Persistence;

public static class AutomatedMigration
{
    private static readonly string[] Roles =
    [
        "Admin",
        "Manager",
        "Franchise Store Staff",
        "Central Kitchen Staff",
        "Supply Coordinator"
    ];

    private const string AdminUserName = "admin";
    private const string AdminEmail    = "admin@kitchen.com";
    private const string AdminPassword = "Admin@123456";

    public static async Task MigrateAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<DatabaseContext>();

        if (context.Database.IsSqlServer()) await context.Database.MigrateAsync();

        await SeedRolesAsync(services);
        await SeedAdminAsync(services);
        await SeedMasterDataAsync(context);
    }

    // ── Auth ────────────────────────────────────────────────────────────────

    private static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task SeedAdminAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByNameAsync(AdminUserName) is not null) return;

        var admin = new ApplicationUser
        {
            UserName       = AdminUserName,
            Email          = AdminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, AdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    // ── Master Data ─────────────────────────────────────────────────────────

    private static async Task SeedMasterDataAsync(DatabaseContext db)
    {
        await SeedCentralKitchensAsync(db);
        await SeedFranchiseStoresAsync(db);
        await SeedProductTypesAsync(db);
        await SeedProductsAsync(db);
        await SeedIngredientsAsync(db);
        await SeedInventoryLocationsAsync(db);
        await SeedRecipesAsync(db);
        await SeedRecipeIngredientsAsync(db);
    }

    private static async Task SeedCentralKitchensAsync(DatabaseContext db)
    {
        if (await db.CentralKitchens.AnyAsync()) return;

        db.CentralKitchens.AddRange(
            new CentralKitchen
            {
                Name      = "Central Kitchen Hanoi",
                Address   = "123 Lang Street, Dong Da, Hanoi",
                Phone     = "024-1234-5678",
                Status    = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CentralKitchen
            {
                Name      = "Central Kitchen Ho Chi Minh",
                Address   = "456 Nguyen Van Linh, District 7, HCMC",
                Phone     = "028-9876-5432",
                Status    = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedFranchiseStoresAsync(DatabaseContext db)
    {
        if (await db.FranchiseStores.AnyAsync()) return;

        db.FranchiseStores.AddRange(
            new FranchiseStore { KitchenId = 1, StoreName = "Cau Giay Branch",     Address = "89 Xuan Thuy, Cau Giay, Hanoi" },
            new FranchiseStore { KitchenId = 1, StoreName = "Hoan Kiem Branch",    Address = "12 Hang Bai, Hoan Kiem, Hanoi" },
            new FranchiseStore { KitchenId = 1, StoreName = "Hai Ba Trung Branch", Address = "55 Lo Duc, Hai Ba Trung, Hanoi" },
            new FranchiseStore { KitchenId = 2, StoreName = "District 1 Branch",   Address = "78 Le Loi, District 1, HCMC" },
            new FranchiseStore { KitchenId = 2, StoreName = "Binh Thanh Branch",   Address = "34 Dinh Tien Hoang, Binh Thanh, HCMC" },
            new FranchiseStore { KitchenId = 2, StoreName = "District 7 Branch",   Address = "201 Nguyen Thi Thap, District 7, HCMC" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedProductTypesAsync(DatabaseContext db)
    {
        if (await db.ProductTypes.AnyAsync()) return;

        db.ProductTypes.AddRange(
            new ProductType { TypeName = "Main Dish",          Description = "Primary dishes on the menu",              StorageCondition = "Room temperature" },
            new ProductType { TypeName = "Sauce & Seasoning",  Description = "Sauces and seasoning accompaniments",     StorageCondition = "Room temperature, keep dry" },
            new ProductType { TypeName = "Beverage",           Description = "Cold and hot drinks",                     StorageCondition = "Refrigerated 2-5C" },
            new ProductType { TypeName = "Frozen Food",        Description = "Frozen raw ingredients",                  StorageCondition = "Frozen -18C" },
            new ProductType { TypeName = "Vegetables & Fruit", Description = "Fresh vegetables and seasonal fruit",     StorageCondition = "Chilled 4-8C" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(DatabaseContext db)
    {
        if (await db.Products.AnyAsync()) return;

        db.Products.AddRange(
            // Main Dish (ProductTypeId = 1)
            new Product { ProductTypeId = 1, ProductName = "Beef Pho Special",          Status = "Active", Unit = "bowl" },
            new Product { ProductTypeId = 1, ProductName = "Hue Beef Noodle Soup",      Status = "Active", Unit = "bowl" },
            new Product { ProductTypeId = 1, ProductName = "Steamed Chicken Rice",      Status = "Active", Unit = "set" },
            new Product { ProductTypeId = 1, ProductName = "Grilled Pork Banh Mi",      Status = "Active", Unit = "piece" },
            new Product { ProductTypeId = 1, ProductName = "Grilled Pork Chop Rice",    Status = "Active", Unit = "set" },
            // Sauce & Seasoning (ProductTypeId = 2)
            new Product { ProductTypeId = 2, ProductName = "Black Bean Sauce",          Status = "Active", Unit = "liter" },
            new Product { ProductTypeId = 2, ProductName = "Sweet and Sour Fish Sauce", Status = "Active", Unit = "liter" },
            new Product { ProductTypeId = 2, ProductName = "Hoisin Sauce",              Status = "Active", Unit = "kg" },
            // Beverage (ProductTypeId = 3)
            new Product { ProductTypeId = 3, ProductName = "Iced Tea",                  Status = "Active", Unit = "glass" },
            new Product { ProductTypeId = 3, ProductName = "Salted Lemonade",           Status = "Active", Unit = "glass" },
            new Product { ProductTypeId = 3, ProductName = "Avocado Smoothie",          Status = "Active", Unit = "glass" },
            // Frozen Food (ProductTypeId = 4)
            new Product { ProductTypeId = 4, ProductName = "Imported Beef",             Status = "Active", Unit = "kg" },
            new Product { ProductTypeId = 4, ProductName = "Frozen Tiger Prawn",        Status = "Active", Unit = "kg" },
            new Product { ProductTypeId = 4, ProductName = "Frozen Pork Hock",          Status = "Active", Unit = "kg" },
            // Vegetables & Fruit (ProductTypeId = 5)
            new Product { ProductTypeId = 5, ProductName = "Morning Glory",             Status = "Active", Unit = "kg" },
            new Product { ProductTypeId = 5, ProductName = "Carrot",                    Status = "Active", Unit = "kg" },
            new Product { ProductTypeId = 5, ProductName = "Spring Onion",              Status = "Active", Unit = "kg" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedIngredientsAsync(DatabaseContext db)
    {
        if (await db.Ingredients.AnyAsync()) return;

        db.Ingredients.AddRange(
            new Ingredient { IngredientName = "Beef",                  Unit = "kg",    StorageCondition = "Chilled 0-4C",               MinStock = 50,  Price = 250000 },
            new Ingredient { IngredientName = "Pork",                  Unit = "kg",    StorageCondition = "Chilled 0-4C",               MinStock = 40,  Price = 150000 },
            new Ingredient { IngredientName = "Whole Chicken",         Unit = "pc",    StorageCondition = "Chilled 0-4C",               MinStock = 30,  Price = 120000 },
            new Ingredient { IngredientName = "Beef Bone",             Unit = "kg",    StorageCondition = "Frozen -18C",                MinStock = 30,  Price = 80000  },
            new Ingredient { IngredientName = "All-purpose Flour",     Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 50,  Price = 20000  },
            new Ingredient { IngredientName = "Jasmine Rice",          Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 100, Price = 25000  },
            new Ingredient { IngredientName = "Fresh Rice Vermicelli", Unit = "kg",    StorageCondition = "Chilled 4-8C",               MinStock = 30,  Price = 30000  },
            new Ingredient { IngredientName = "Fresh Pho Noodle",      Unit = "kg",    StorageCondition = "Chilled 4-8C",               MinStock = 30,  Price = 35000  },
            new Ingredient { IngredientName = "Onion",                 Unit = "kg",    StorageCondition = "Room temperature",            MinStock = 20,  Price = 20000  },
            new Ingredient { IngredientName = "Garlic",                Unit = "kg",    StorageCondition = "Room temperature",            MinStock = 10,  Price = 40000  },
            new Ingredient { IngredientName = "Lemongrass",            Unit = "kg",    StorageCondition = "Chilled 4-8C",               MinStock = 10,  Price = 30000  },
            new Ingredient { IngredientName = "Ginger",                Unit = "kg",    StorageCondition = "Room temperature",            MinStock = 5,   Price = 50000  },
            new Ingredient { IngredientName = "Fish Sauce",            Unit = "liter", StorageCondition = "Room temperature",            MinStock = 20,  Price = 45000  },
            new Ingredient { IngredientName = "Cooking Oil",           Unit = "liter", StorageCondition = "Room temperature",            MinStock = 20,  Price = 40000  },
            new Ingredient { IngredientName = "White Sugar",           Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 20,  Price = 25000  },
            new Ingredient { IngredientName = "Salt",                  Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 10,  Price = 10000  },
            new Ingredient { IngredientName = "Seasoning Powder",      Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 10,  Price = 50000  },
            new Ingredient { IngredientName = "Black Pepper",          Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 5,   Price = 200000 }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedInventoryLocationsAsync(DatabaseContext db)
    {
        if (await db.InventoryLocations.AnyAsync()) return;

        db.InventoryLocations.AddRange(
            // Hanoi (CentralKitchenId = 1)
            new InventoryLocation { CentralKitchenId = 1, Name = "Cold Room HN-A",  LocationType = "Cold Storage", Status = "Active", UpdatedAt = DateTime.UtcNow },
            new InventoryLocation { CentralKitchenId = 1, Name = "Dry Store HN-A",  LocationType = "Dry Storage",  Status = "Active", UpdatedAt = DateTime.UtcNow },
            new InventoryLocation { CentralKitchenId = 1, Name = "Freezer HN-A",    LocationType = "Freezer",      Status = "Active", UpdatedAt = DateTime.UtcNow },
            // HCMC (CentralKitchenId = 2)
            new InventoryLocation { CentralKitchenId = 2, Name = "Cold Room HCM-A", LocationType = "Cold Storage", Status = "Active", UpdatedAt = DateTime.UtcNow },
            new InventoryLocation { CentralKitchenId = 2, Name = "Dry Store HCM-A", LocationType = "Dry Storage",  Status = "Active", UpdatedAt = DateTime.UtcNow },
            new InventoryLocation { CentralKitchenId = 2, Name = "Freezer HCM-A",   LocationType = "Freezer",      Status = "Active", UpdatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedRecipesAsync(DatabaseContext db)
    {
        if (await db.Recipes.AnyAsync()) return;

        db.Recipes.AddRange(
            new Recipe { RecipeName = "Beef Pho Special",          Description = "Beef bone broth simmered 8 hours, rare and well-done beef slices, spring onion, coriander",                   CreatedDate = DateTime.UtcNow },
            new Recipe { RecipeName = "Hue Beef Noodle Soup",      Description = "Spicy lemongrass broth with pork hock, crab paste cake and shrimp paste",                                     CreatedDate = DateTime.UtcNow },
            new Recipe { RecipeName = "Steamed Chicken Rice",      Description = "Ginger-lemongrass steamed chicken, jasmine rice cooked in chicken broth, ginger dipping sauce",               CreatedDate = DateTime.UtcNow },
            new Recipe { RecipeName = "Grilled Pork Banh Mi",      Description = "Crispy baguette, charcoal-grilled pork, pickled carrot & daikon, fresh herbs, pate",                          CreatedDate = DateTime.UtcNow },
            new Recipe { RecipeName = "Grilled Pork Chop Rice",    Description = "Lemongrass-chili marinated pork chop, steamed rice, scallion oil, pickled vegetables",                        CreatedDate = DateTime.UtcNow },
            new Recipe { RecipeName = "Black Bean Sauce",          Description = "Fried garlic black bean sauce with sesame oil and chicken stock — served with chicken rice",                   CreatedDate = DateTime.UtcNow },
            new Recipe { RecipeName = "Sweet and Sour Fish Sauce", Description = "Balanced fish sauce, lime, sugar, garlic and chili dipping sauce for pho and noodle dishes",                  CreatedDate = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedRecipeIngredientsAsync(DatabaseContext db)
    {
        if (await db.RecipeIngredients.AnyAsync()) return;

        // Load actual IDs from DB by name — safe regardless of IDENTITY counter value
        var recipes = await db.Recipes.ToDictionaryAsync(r => r.RecipeName, r => r.RecipeId);
        var ings    = await db.Ingredients.ToDictionaryAsync(i => i.IngredientName, i => i.IngredientId);

        void Add(string recipeName, string ingName, decimal qty)
        {
            if (recipes.TryGetValue(recipeName, out var rid) && ings.TryGetValue(ingName, out var iid))
                db.RecipeIngredients.Add(new RecipeIngredient { RecipeId = rid, IngredientId = iid, Quantity = qty });
        }

        // ── Beef Pho Special ────────────────────────────────────────────────────
        Add("Beef Pho Special",         "Beef",                  0.3m);
        Add("Beef Pho Special",         "Beef Bone",             0.5m);
        Add("Beef Pho Special",         "Fresh Pho Noodle",      0.2m);
        Add("Beef Pho Special",         "Onion",                 0.05m);
        Add("Beef Pho Special",         "Ginger",                0.02m);
        Add("Beef Pho Special",         "Fish Sauce",            0.02m);
        Add("Beef Pho Special",         "Salt",                  0.005m);
        Add("Beef Pho Special",         "Black Pepper",          0.002m);

        // ── Hue Beef Noodle Soup ────────────────────────────────────────────────
        Add("Hue Beef Noodle Soup",     "Pork",                  0.2m);
        Add("Hue Beef Noodle Soup",     "Beef",                  0.1m);
        Add("Hue Beef Noodle Soup",     "Fresh Rice Vermicelli", 0.2m);
        Add("Hue Beef Noodle Soup",     "Lemongrass",            0.02m);
        Add("Hue Beef Noodle Soup",     "Ginger",                0.01m);
        Add("Hue Beef Noodle Soup",     "Fish Sauce",            0.02m);
        Add("Hue Beef Noodle Soup",     "Seasoning Powder",      0.01m);

        // ── Steamed Chicken Rice ────────────────────────────────────────────────
        Add("Steamed Chicken Rice",     "Whole Chicken",         0.5m);
        Add("Steamed Chicken Rice",     "Jasmine Rice",          0.15m);
        Add("Steamed Chicken Rice",     "Ginger",                0.02m);
        Add("Steamed Chicken Rice",     "Lemongrass",            0.01m);
        Add("Steamed Chicken Rice",     "Fish Sauce",            0.02m);
        Add("Steamed Chicken Rice",     "Salt",                  0.005m);

        // ── Grilled Pork Banh Mi ────────────────────────────────────────────────
        Add("Grilled Pork Banh Mi",     "Pork",                  0.15m);
        Add("Grilled Pork Banh Mi",     "All-purpose Flour",     0.1m);
        Add("Grilled Pork Banh Mi",     "Onion",                 0.03m);
        Add("Grilled Pork Banh Mi",     "Garlic",                0.02m);
        Add("Grilled Pork Banh Mi",     "Fish Sauce",            0.01m);
        Add("Grilled Pork Banh Mi",     "Salt",                  0.003m);

        // ── Grilled Pork Chop Rice ──────────────────────────────────────────────
        Add("Grilled Pork Chop Rice",   "Pork",                  0.2m);
        Add("Grilled Pork Chop Rice",   "Jasmine Rice",          0.15m);
        Add("Grilled Pork Chop Rice",   "Lemongrass",            0.01m);
        Add("Grilled Pork Chop Rice",   "Cooking Oil",           0.02m);
        Add("Grilled Pork Chop Rice",   "Fish Sauce",            0.01m);
        Add("Grilled Pork Chop Rice",   "Seasoning Powder",      0.005m);

        // ── Black Bean Sauce ────────────────────────────────────────────────────
        Add("Black Bean Sauce",         "Garlic",                0.05m);
        Add("Black Bean Sauce",         "Cooking Oil",           0.05m);
        Add("Black Bean Sauce",         "Seasoning Powder",      0.01m);
        Add("Black Bean Sauce",         "Salt",                  0.005m);

        // ── Sweet and Sour Fish Sauce ───────────────────────────────────────────
        Add("Sweet and Sour Fish Sauce","Fish Sauce",            0.1m);
        Add("Sweet and Sour Fish Sauce","White Sugar",           0.02m);
        Add("Sweet and Sour Fish Sauce","Salt",                  0.005m);

        await db.SaveChangesAsync();
    }
}
