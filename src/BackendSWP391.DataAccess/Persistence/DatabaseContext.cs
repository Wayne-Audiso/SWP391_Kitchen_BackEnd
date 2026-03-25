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

    public virtual DbSet<StoreOrder>          StoreOrders          { get; set; }

    public virtual DbSet<StoreOrderLine>      StoreOrderLines      { get; set; }

    public virtual DbSet<ProductionBatch>     ProductionBatches    { get; set; }

    public virtual DbSet<ProductionBatchLine> ProductionBatchLines { get; set; }

    public virtual DbSet<StoreIngredientStock> StoreIngredientStocks { get; set; }

    public virtual DbSet<StoreCostRecord>      StoreCostRecords      { get; set; }

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
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
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
                .HasColumnName("address");
            entity.Property(e => e.KitchenId).HasColumnName("kitchen_id");
            entity.Property(e => e.StoreName)
                .IsRequired()
                .HasMaxLength(100)
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
                .HasColumnName("ingredientName");
            entity.Property(e => e.StorageCondition)
                .HasMaxLength(100)
                .HasColumnName("storageCondition");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.MinStock)
                .HasColumnName("minStock");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("price");
            entity.Property(e => e.CurrentStock)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("currentStock");
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
                .HasColumnName("productName");
            entity.Property(e => e.ProductTypeId).HasColumnName("productTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

            entity.Property(e => e.RecipeId).HasColumnName("recipeID");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_type");

            entity.HasOne(d => d.Recipe).WithMany()
                .HasForeignKey(d => d.RecipeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_product_recipe");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__CA28F43E35BBD312");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProductTypeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("productTypeID");
            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("description");
            entity.Property(e => e.StorageCondition)
                .HasMaxLength(100)
                .HasColumnName("storageCondition");
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(100)
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
                .HasColumnType("nvarchar(max)")
                .HasColumnName("description");
            entity.Property(e => e.RecipeName)
                .IsRequired()
                .HasMaxLength(100)
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
            entity.Property(e => e.RejectReason)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("rejectReason");
            entity.Property(e => e.ProductionBatchId).HasColumnName("productionBatchID");

            entity.HasOne(d => d.ProductionBatch).WithMany(p => p.StoreOrders)
                .HasForeignKey(d => d.ProductionBatchId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_order_batch");

            entity.HasOne(d => d.CentralKitchen).WithMany(p => p.StoreOrders)
                .HasForeignKey(d => d.CentralKitchenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_kitchen");

            entity.HasOne(d => d.FranchiseStore).WithMany(p => p.StoreOrders)
                .HasForeignKey(d => d.FranchiseStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_store");
        });

        modelBuilder.Entity<StoreOrderLine>(entity =>
        {
            entity.HasKey(e => e.StoreOrderLineId).HasName("PK__StoreOrdLine");

            entity.ToTable("StoreOrderLine");

            entity.Property(e => e.StoreOrderLineId)
                .ValueGeneratedOnAdd()
                .HasColumnName("storeOrderLineID");
            entity.Property(e => e.StoreOrderId).HasColumnName("storeOrderID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.StoreOrder).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.StoreOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orderline_order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orderline_product");
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

        modelBuilder.Entity<ProductionBatch>(entity =>
        {
            entity.HasKey(e => e.ProductionBatchId);

            entity.ToTable("ProductionBatch");

            entity.Property(e => e.ProductionBatchId)
                .ValueGeneratedOnAdd()
                .HasColumnName("productionBatchID");
            entity.Property(e => e.CentralKitchenId).HasColumnName("centralKitchenID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.CompletedDate)
                .HasColumnType("datetime")
                .HasColumnName("completedDate");
            entity.Property(e => e.Notes)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("notes");

            entity.HasOne(d => d.CentralKitchen).WithMany(p => p.ProductionBatches)
                .HasForeignKey(d => d.CentralKitchenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_batch_kitchen");
        });

        modelBuilder.Entity<ProductionBatchLine>(entity =>
        {
            entity.HasKey(e => e.ProductionBatchLineId);

            entity.ToTable("ProductionBatchLine");

            entity.Property(e => e.ProductionBatchLineId)
                .ValueGeneratedOnAdd()
                .HasColumnName("productionBatchLineID");
            entity.Property(e => e.ProductionBatchId).HasColumnName("productionBatchID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.RequiredQuantity).HasColumnName("requiredQuantity");
            entity.Property(e => e.ProducedQuantity).HasColumnName("producedQuantity");

            entity.HasOne(d => d.ProductionBatch).WithMany(p => p.Lines)
                .HasForeignKey(d => d.ProductionBatchId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_batchline_batch");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_batchline_product");
        });

        modelBuilder.Entity<StoreIngredientStock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("StoreIngredientStock");

            entity.Property(e => e.CurrentStock)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.StoreId, e.IngredientId })
                .HasDatabaseName("IX_StoreIngredientStock_Store");

            entity.HasOne(d => d.Store)
                .WithMany()
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_sis_store");

            entity.HasOne(d => d.Ingredient)
                .WithMany()
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_sis_ingredient");
        });

        modelBuilder.Entity<StoreCostRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("StoreCostRecord");

            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Cost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CostType).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Store)
                .WithMany()
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_scr_store");

            entity.HasOne(d => d.Ingredient)
                .WithMany()
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_scr_ingredient");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
