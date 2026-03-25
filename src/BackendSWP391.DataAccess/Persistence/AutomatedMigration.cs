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

    private record SeedUser(string UserName, string Email, string Password, string Role);

    private static readonly SeedUser[] SeedUsers =
    [
        new("admin",        "admin@kitchen.com",    "Admin@123456",   "Admin"),
        new("manager",      "manager@kitchen.com",  "Manager@123456", "Manager"),
        new("store_staff",  "store@kitchen.com",    "Store@123456",   "Franchise Store Staff"),
        new("kitchen_staff","kitchen@kitchen.com",  "Kitchen@123456", "Central Kitchen Staff"),
        new("supply_coord", "supply@kitchen.com",   "Supply@123456",  "Supply Coordinator"),
    ];

    public static async Task MigrateAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<DatabaseContext>();

        if (context.Database.IsSqlServer()) await context.Database.MigrateAsync();

        await SeedRolesAsync(services);
        await SeedUsersAsync(services);
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

    private static async Task SeedUsersAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var seed in SeedUsers)
        {
            if (await userManager.FindByNameAsync(seed.UserName) is not null) continue;

            var user = new ApplicationUser
            {
                UserName       = seed.UserName,
                Email          = seed.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, seed.Password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, seed.Role);
        }
    }

    // ── Master Data ─────────────────────────────────────────────────────────

    private static async Task SeedMasterDataAsync(DatabaseContext db)
    {
        await SeedCentralKitchensAsync(db);
        await SeedFranchiseStoresAsync(db);
        await SeedProductTypesAsync(db);
        await SeedProductsAsync(db);
        await SeedIngredientsAsync(db);
        await NormalizeIngredientCurrentStockAsync(db);
        await SeedInventoryLocationsAsync(db);
        await SeedRecipesAsync(db);
        await SeedRecipeIngredientsAsync(db);
        await SeedProductRecipeLinksAsync(db);
        await SeedStoreIngredientStocksAsync(db);
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

        // RecipeId sẽ được gán sau khi Recipes được seed (SeedProductRecipeLinksAsync)
        db.Products.AddRange(
            // Main Dish (ProductTypeId = 1) — RecipeId gán sau
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
            new Ingredient { IngredientName = "Beef",                  Unit = "kg",    StorageCondition = "Chilled 0-4C",               MinStock = 50,  Price = 250000, CurrentStock = 150  },
            new Ingredient { IngredientName = "Pork",                  Unit = "kg",    StorageCondition = "Chilled 0-4C",               MinStock = 40,  Price = 150000, CurrentStock = 120  },
            new Ingredient { IngredientName = "Whole Chicken",         Unit = "pc",    StorageCondition = "Chilled 0-4C",               MinStock = 30,  Price = 120000, CurrentStock = 80   },
            new Ingredient { IngredientName = "Beef Bone",             Unit = "kg",    StorageCondition = "Frozen -18C",                MinStock = 30,  Price = 80000,  CurrentStock = 100  },
            new Ingredient { IngredientName = "All-purpose Flour",     Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 50,  Price = 20000,  CurrentStock = 200  },
            new Ingredient { IngredientName = "Jasmine Rice",          Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 100, Price = 25000,  CurrentStock = 300  },
            new Ingredient { IngredientName = "Fresh Rice Vermicelli", Unit = "kg",    StorageCondition = "Chilled 4-8C",               MinStock = 30,  Price = 30000,  CurrentStock = 80   },
            new Ingredient { IngredientName = "Fresh Pho Noodle",      Unit = "kg",    StorageCondition = "Chilled 4-8C",               MinStock = 30,  Price = 35000,  CurrentStock = 90   },
            new Ingredient { IngredientName = "Onion",                 Unit = "kg",    StorageCondition = "Room temperature",           MinStock = 20,  Price = 20000,  CurrentStock = 60   },
            new Ingredient { IngredientName = "Garlic",                Unit = "kg",    StorageCondition = "Room temperature",           MinStock = 10,  Price = 40000,  CurrentStock = 30   },
            new Ingredient { IngredientName = "Lemongrass",            Unit = "kg",    StorageCondition = "Chilled 4-8C",               MinStock = 10,  Price = 30000,  CurrentStock = 25   },
            new Ingredient { IngredientName = "Ginger",                Unit = "kg",    StorageCondition = "Room temperature",           MinStock = 5,   Price = 50000,  CurrentStock = 15   },
            new Ingredient { IngredientName = "Fish Sauce",            Unit = "liter", StorageCondition = "Room temperature",           MinStock = 20,  Price = 45000,  CurrentStock = 50   },
            new Ingredient { IngredientName = "Cooking Oil",           Unit = "liter", StorageCondition = "Room temperature",           MinStock = 20,  Price = 40000,  CurrentStock = 60   },
            new Ingredient { IngredientName = "White Sugar",           Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 20,  Price = 25000,  CurrentStock = 50   },
            new Ingredient { IngredientName = "Salt",                  Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 10,  Price = 10000,  CurrentStock = 30   },
            new Ingredient { IngredientName = "Seasoning Powder",      Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 10,  Price = 50000,  CurrentStock = 25   },
            new Ingredient { IngredientName = "Black Pepper",          Unit = "kg",    StorageCondition = "Room temperature, keep dry", MinStock = 5,   Price = 200000, CurrentStock = 15   }
        );
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Đảm bảo tồn kho nguyên liệu bếp trung tâm không bị NULL.
    /// Tránh trường hợp DB đã có Ingredient từ trước (seed không chạy) dẫn tới CurrentStock = NULL.
    /// </summary>
    private static async Task NormalizeIngredientCurrentStockAsync(DatabaseContext db)
    {
        // EF Core 7+ supports ExecuteUpdateAsync
        var affected = await db.Ingredients
            .Where(i => i.CurrentStock == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(
                i => i.CurrentStock,
                i => (decimal)(
                    (i.MinStock.HasValue && i.MinStock.Value > 0)
                        ? (i.MinStock.Value * 3)  // đủ để test luồng giao từ kho
                        : 100                     // fallback nếu chưa cấu hình MinStock
                )));

        _ = affected;
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

    /// <summary>
    /// Gắn Recipe vào Product sau khi cả hai đã được seed.
    /// Chỉ update những product chưa có RecipeId.
    /// </summary>
    private static async Task SeedProductRecipeLinksAsync(DatabaseContext db)
    {
        var products = await db.Products.ToDictionaryAsync(p => p.ProductName, p => p);
        var recipes  = await db.Recipes.ToDictionaryAsync(r => r.RecipeName,   r => r.RecipeId);

        var links = new Dictionary<string, string>
        {
            ["Beef Pho Special"]          = "Beef Pho Special",
            ["Hue Beef Noodle Soup"]      = "Hue Beef Noodle Soup",
            ["Steamed Chicken Rice"]      = "Steamed Chicken Rice",
            ["Grilled Pork Banh Mi"]      = "Grilled Pork Banh Mi",
            ["Grilled Pork Chop Rice"]    = "Grilled Pork Chop Rice",
            ["Black Bean Sauce"]          = "Black Bean Sauce",
            ["Sweet and Sour Fish Sauce"] = "Sweet and Sour Fish Sauce",
        };

        var changed = false;
        foreach (var (productName, recipeName) in links)
        {
            if (!products.TryGetValue(productName, out var product)) continue;
            if (!recipes.TryGetValue(recipeName,   out var recipeId)) continue;
            if (product.RecipeId == recipeId) continue;

            product.RecipeId = recipeId;
            changed = true;
        }

        if (changed) await db.SaveChangesAsync();
    }

    /// <summary>
    /// Seed kho nguyên liệu mẫu cho 2 cửa hàng đầu tiên để test luồng bán hàng.
    /// Hạn sử dụng: lô A còn hạn (30 ngày), lô B sắp hết hạn (2 ngày) để test ProcessExpired.
    /// </summary>
    private static async Task SeedStoreIngredientStocksAsync(DatabaseContext db)
    {
        if (await db.StoreIngredientStocks.AnyAsync()) return;

        var now    = DateTime.UtcNow;
        var soon   = now.AddDays(2);   // sắp hết hạn — test ProcessExpired
        var normal = now.AddDays(30);  // còn hàng bình thường

        // ── Ingredient IDs (theo thứ tự seed) ───────────────────────────────────
        // 1=Beef, 2=Pork, 3=WholeChicken, 4=BeefBone, 5=Flour, 6=JasmineRice,
        // 7=FreshRiceVermicelli, 8=FreshPhoNoodle, 9=Onion, 10=Garlic,
        // 11=Lemongrass, 12=Ginger, 13=FishSauce, 14=CookingOil,
        // 15=WhiteSugar, 16=Salt, 17=SeasoningPowder, 18=BlackPepper

        var ings = await db.Ingredients.ToDictionaryAsync(i => i.IngredientName, i => i.IngredientId);

        int? Id(string name) => ings.TryGetValue(name, out var id) ? id : null;

        void Stock(int storeId, string ingName, decimal qty, DateTime expiry)
        {
            var ingId = Id(ingName);
            if (ingId is null) return;
            db.StoreIngredientStocks.Add(new StoreIngredientStock
            {
                StoreId      = storeId,
                IngredientId = ingId.Value,
                CurrentStock = qty,
                ExpiryDate   = expiry,
                UpdatedAt    = now
            });
        }

        // ── Store 1 (Cau Giay Branch) — đủ nguyên liệu cho nhiều món ─────────────
        // Lô bình thường
        Stock(1, "Beef",                  5m,   normal);
        Stock(1, "Beef Bone",             8m,   normal);
        Stock(1, "Fresh Pho Noodle",      4m,   normal);
        Stock(1, "Pork",                  5m,   normal);
        Stock(1, "Fresh Rice Vermicelli", 4m,   normal);
        Stock(1, "Whole Chicken",         10m,  normal);
        Stock(1, "Jasmine Rice",          10m,  normal);
        Stock(1, "All-purpose Flour",     5m,   normal);
        Stock(1, "Onion",                 3m,   normal);
        Stock(1, "Garlic",                2m,   normal);
        Stock(1, "Lemongrass",            2m,   normal);
        Stock(1, "Ginger",                1m,   normal);
        Stock(1, "Fish Sauce",            3m,   normal);
        Stock(1, "Cooking Oil",           3m,   normal);
        Stock(1, "White Sugar",           2m,   normal);
        Stock(1, "Salt",                  1m,   normal);
        Stock(1, "Seasoning Powder",      1m,   normal);
        Stock(1, "Black Pepper",          0.5m, normal);
        // Lô sắp hết hạn — để test "Kiểm tra hàng hết hạn"
        Stock(1, "Beef",                  2m,   soon);
        Stock(1, "Fresh Pho Noodle",      1m,   soon);

        // ── Store 2 (Hoan Kiem Branch) — kho ít hàng để test cảnh báo tồn kho thấp
        Stock(2, "Beef",                  0.2m, normal); // dưới MinStock (50kg)
        Stock(2, "Beef Bone",             0.3m, normal);
        Stock(2, "Fresh Pho Noodle",      0.5m, normal);
        Stock(2, "Jasmine Rice",          2m,   normal);
        Stock(2, "Whole Chicken",         1m,   normal);
        Stock(2, "Fish Sauce",            0.5m, normal);
        Stock(2, "Salt",                  0.2m, normal);
        Stock(2, "Ginger",                0.1m, normal);
        Stock(2, "Lemongrass",            0.1m, normal);

        await db.SaveChangesAsync();
    }
}
