using Microsoft.EntityFrameworkCore;
using BackendSWP391.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BackendSWP391.DataAccess.Identity;

namespace BackendSWP391.DataAccess.Persistence;

public partial class DatabaseContext : IdentityDbContext<ApplicationUser>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CentralKitchen> CentralKitchens { get; set; }

    public virtual DbSet<FranchiseStore> FranchiseStores { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<InventoryLocation> InventoryLocations { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<ShipmentLine> ShipmentLines { get; set; }

    public virtual DbSet<StoreOrder> StoreOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CentralKitchen>(entity =>
        {
            entity.HasKey(e => e.CentralKitchenId).HasName("PK__CentralK__C1E5E783FE9495EC");

            entity.ToTable("CentralKitchen");

            entity.Property(e => e.CentralKitchenId)
                .ValueGeneratedOnAdd()
                .HasColumnName("centralKitchenID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<FranchiseStore>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK__Franchis__A2F2A30C21DEC58E");

            entity.ToTable("FranchiseStore");

            entity.Property(e => e.StoreId)
                .ValueGeneratedOnAdd()
                .HasColumnName("store_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.KitchenId).HasColumnName("kitchen_id");
            entity.Property(e => e.StoreName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("store_name");

            entity.HasOne(d => d.Kitchen).WithMany(p => p.FranchiseStores)
                .HasForeignKey(d => d.KitchenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_store_kitchen");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__2753A6C7152CB844");

            entity.ToTable("Ingredient");

            entity.Property(e => e.IngredientId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ingredientID");
            entity.Property(e => e.IngredientName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ingredientName");
            entity.Property(e => e.StorageCondition)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("storageCondition");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("unit");
            entity.Property(e => e.MinStock)
                .HasColumnName("minStock");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<InventoryLocation>(entity =>
        {
            entity.HasKey(e => e.InventoryLocationId).HasName("PK__Inventor__A2A6C2F0FC025C81");

            entity.ToTable("InventoryLocation");

            entity.Property(e => e.InventoryLocationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("inventoryLocationID");
            entity.Property(e => e.CentralKitchenId).HasColumnName("centralKitchenID");
            entity.Property(e => e.LocationType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("location_type");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CentralKitchen).WithMany(p => p.InventoryLocations)
                .HasForeignKey(d => d.CentralKitchenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_inventory_kitchen");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__2D10D14ACEDA15FA");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("productID");
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("productName");
            entity.Property(e => e.ProductTypeId).HasColumnName("productTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("unit");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_type");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__CA28F43E35BBD312");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProductTypeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("productTypeID");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.StorageCondition)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("storageCondition");
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("typeName");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipe__C114EE63FBCAA5D0");

            entity.ToTable("Recipe");

            entity.Property(e => e.RecipeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("recipeID");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.RecipeName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("recipeName");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.ShipmentId).HasName("PK__Shipment__47217FE16B7B71B7");

            entity.ToTable("Shipment");

            entity.Property(e => e.ShipmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("shipmentID");
            entity.Property(e => e.CentralKitchenId).HasColumnName("centralKitchenID");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("deliveryStatus");
            entity.Property(e => e.ReceivedDate)
                .HasColumnType("datetime")
                .HasColumnName("receivedDate");
            entity.Property(e => e.ShipmentDate)
                .HasColumnType("datetime")
                .HasColumnName("shipmentDate");
            entity.Property(e => e.StoreOrderId).HasColumnName("storeOrderID");

            entity.HasOne(d => d.CentralKitchen).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.CentralKitchenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shipment_kitchen");

            entity.HasOne(d => d.StoreOrder).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.StoreOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shipment_order");
        });

        modelBuilder.Entity<ShipmentLine>(entity =>
        {
            entity.HasKey(e => e.ShipmentLineId).HasName("PK__Shipment__32DEEEA9A317585B");

            entity.ToTable("ShipmentLine");

            entity.Property(e => e.ShipmentLineId)
                .ValueGeneratedOnAdd()
                .HasColumnName("shipmentLineID");
            entity.Property(e => e.DamagedQuantity).HasColumnName("damagedQuantity");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.ReceivedQuantity).HasColumnName("receivedQuantity");
            entity.Property(e => e.ShipmentId).HasColumnName("shipmentID");
            entity.Property(e => e.ShippedQuantity).HasColumnName("shippedQuantity");

            entity.HasOne(d => d.Product).WithMany(p => p.ShipmentLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_line_product");

            entity.HasOne(d => d.Shipment).WithMany(p => p.ShipmentLines)
                .HasForeignKey(d => d.ShipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_line_shipment");
        });

        modelBuilder.Entity<StoreOrder>(entity =>
        {
            entity.HasKey(e => e.StoreOrderId).HasName("PK__StoreOrd__3353A2C6E34AC3AD");

            entity.ToTable("StoreOrder");

            entity.Property(e => e.StoreOrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("storeOrderID");
            entity.Property(e => e.CentralKitchenId).HasColumnName("centralKitchenID");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("deliveryDate");
            entity.Property(e => e.FranchiseStoreId).HasColumnName("franchiseStoreID");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("orderDate");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.CentralKitchen).WithMany(p => p.StoreOrders)
                .HasForeignKey(d => d.CentralKitchenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_kitchen");

            entity.HasOne(d => d.FranchiseStore).WithMany(p => p.StoreOrders)
                .HasForeignKey(d => d.FranchiseStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_store");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.RecipeIngredientId);

            entity.ToTable("RecipeIngredient");

            entity.Property(e => e.RecipeIngredientId)
                .ValueGeneratedOnAdd()
                .HasColumnName("recipeIngredientID");
            entity.Property(e => e.RecipeId).HasColumnName("recipeID");
            entity.Property(e => e.IngredientId).HasColumnName("ingredientID");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18,4)")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Recipe)
                .WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_ri_recipe");

            entity.HasOne(d => d.Ingredient)
                .WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ri_ingredient");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
