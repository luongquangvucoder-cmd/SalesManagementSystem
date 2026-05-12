using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sales_Management_System_API.Models;

namespace Sales_Management_System_API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        //public DbSet<Attribute> Attributes { get; set; }
        //public DbSet<AttributeValue> AttributeValues { get; set; }
        //public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>().ToTable("RefreshTokens");

            builder.Entity<RefreshToken>()
                .HasKey(rt => rt.Id);

            builder.Entity<RefreshToken>()
                .Property(rt => rt.Token)
                .HasMaxLength(500)
                .IsRequired();

            builder.Entity<RefreshToken>()
                .Property(rt => rt.JwtId)
                .HasMaxLength(200)
                .IsRequired();

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId);

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>().ToTable("Categories");

            builder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            builder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Category>()
                .Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Category>()
                .HasIndex(c => c.Name);

            builder.Entity<Category>()
                .HasIndex(c => c.ParentId);

            builder.Entity<Category>()
                .HasIndex(c => new { c.ParentId, c.IsActive });

            builder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>().ToTable("Products");

            builder.Entity<Product>()
                .HasKey(p => p.ProductId);

            builder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Entity<Product>()
                .Property(p => p.Brand)
                .HasMaxLength(100);

            builder.Entity<Product>()
                .Property(p => p.Description)
                .HasMaxLength(4000);

            builder.Entity<Product>()
                .Property(p => p.Status)
                .HasDefaultValue(true);

            builder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            builder.Entity<Product>()
                .HasIndex(p => p.Name);

            builder.Entity<Product>()
                .HasIndex(p => p.Brand);

            builder.Entity<Product>()
                .HasIndex(p => new { p.CategoryId, p.Status });

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductVariant>().ToTable("ProductVariants");

            builder.Entity<ProductVariant>()
                .HasKey(pv => pv.ProductVariantId);

            builder.Entity<ProductVariant>()
                .Property(pv => pv.SKU)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Entity<ProductVariant>()
                .Property(pv => pv.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<ProductVariant>()
                .HasIndex(pv => pv.SKU)
                .IsUnique();

            builder.Entity<ProductVariant>()
                .HasIndex(pv => pv.ProductId);

            builder.Entity<ProductVariant>()
                .HasIndex(pv => new { pv.ProductId, pv.StockQuantity });

            builder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductImage>().ToTable("ProductImages");

            builder.Entity<ProductImage>()
                .HasKey(pi => pi.ProductImageId);

            builder.Entity<ProductImage>()
                .Property(pi => pi.ImageUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Entity<ProductImage>()
                .HasIndex(pi => pi.ProductId);

            builder.Entity<ProductImage>()
                .HasIndex(pi => new { pi.ProductId, pi.IsPrimary });

            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //// =========================================================
            //// ATTRIBUTE
            //// =========================================================
            //builder.Entity<Attribute>().ToTable("Attributes");

            //builder.Entity<Attribute>()
            //    .HasKey(a => a.AttributeId);

            //builder.Entity<Attribute>()
            //    .Property(a => a.Name)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //builder.Entity<Attribute>()
            //    .HasIndex(a => a.Name)
            //    .IsUnique();

            //// =========================================================
            //// ATTRIBUTE VALUE
            //// =========================================================
            //builder.Entity<AttributeValue>().ToTable("AttributeValues");

            //builder.Entity<AttributeValue>()
            //    .HasKey(av => av.AttributeValueId);

            //builder.Entity<AttributeValue>()
            //    .Property(av => av.Value)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //builder.Entity<AttributeValue>()
            //    .HasIndex(av => av.AttributeId);

            //builder.Entity<AttributeValue>()
            //    .HasIndex(av => new { av.AttributeId, av.Value });

            //builder.Entity<AttributeValue>()
            //    .HasOne(av => av.Attribute)
            //    .WithMany(a => a.Values)
            //    .HasForeignKey(av => av.AttributeId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// =========================================================
            //// PRODUCT VARIANT ATTRIBUTE
            //// =========================================================
            //builder.Entity<ProductVariantAttribute>()
            //    .ToTable("ProductVariantAttributes");

            //builder.Entity<ProductVariantAttribute>()
            //    .HasKey(pva => new
            //    {
            //        pva.ProductVariantId,
            //        pva.AttributeValueId
            //    });

            //builder.Entity<ProductVariantAttribute>()
            //    .HasIndex(pva => pva.ProductVariantId);

            //builder.Entity<ProductVariantAttribute>()
            //    .HasIndex(pva => pva.AttributeValueId);

            //builder.Entity<ProductVariantAttribute>()
            //    .HasOne(pva => pva.ProductVariant)
            //    .WithMany(pv => pv.ProductVariantAttributes)
            //    .HasForeignKey(pva => pva.ProductVariantId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<ProductVariantAttribute>()
            //    .HasOne(pva => pva.AttributeValue)
            //    .WithMany(av => av.ProductVariantAttributes)
            //    .HasForeignKey(pva => pva.AttributeValueId)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>().ToTable("Carts");

            builder.Entity<Cart>()
                .HasKey(c => c.CartId);

            builder.Entity<Cart>()
                .Property(c => c.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Entity<Cart>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>().ToTable("CartItems");

            builder.Entity<CartItem>()
                .HasKey(ci => ci.CartItemId);

            builder.Entity<CartItem>()
                .Property(ci => ci.Quantity)
                .IsRequired();

            builder.Entity<CartItem>()
                .HasIndex(ci => ci.CartId);

            builder.Entity<CartItem>()
                .HasIndex(ci => ci.ProductVariantId);

            builder.Entity<CartItem>()
                .HasIndex(ci => new
                {
                    ci.CartId,
                    ci.ProductVariantId
                }).IsUnique();

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>().ToTable("Orders");

            builder.Entity<Order>()
                .HasKey(o => o.OrderId);

            builder.Entity<Order>()
                .Property(o => o.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.OrderCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.ShippingAddress)
                .HasMaxLength(500)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.ReceiverName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.ReceiverPhone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Entity<Order>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Order>()
                .HasIndex(o => o.UserId);

            builder.Entity<Order>()
                .HasIndex(o => o.OrderStatus);

            builder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderItem>().ToTable("OrderItems");

            builder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemId);

            builder.Entity<OrderItem>()
                .Property(oi => oi.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Entity<OrderItem>()
                .Property(oi => oi.SKU)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Entity<OrderItem>()
                .Property(oi => oi.Quantity)
                .IsRequired();

            builder.Entity<OrderItem>()
                .Property(oi => oi.SubTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Entity<OrderItem>()
                .HasIndex(oi => oi.OrderId);

            builder.Entity<OrderItem>()
                .HasIndex(oi => oi.ProductVariantId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>().ToTable("Payments");

            builder.Entity<Payment>()
                .HasKey(p => p.PaymentId);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Entity<Payment>()
                .Property(p => p.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Payment>()
                .Property(p => p.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Payment>()
                .Property(p => p.TransactionCode)
                .HasMaxLength(200);

            builder.Entity<Payment>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Payment>()
                .HasIndex(p => p.OrderId);

            builder.Entity<Payment>()
                .HasIndex(p => p.TransactionCode)
                .IsUnique();

            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Address>().ToTable("Addresses");

            builder.Entity<Address>()
                .HasKey(a => a.AddressId);

            builder.Entity<Address>()
                .Property(a => a.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Entity<Address>()
                .Property(a => a.FullName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Address>()
                .Property(a => a.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Entity<Address>()
                .Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Address>()
                .Property(a => a.District)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Address>()
                .Property(a => a.Ward)
                .HasMaxLength(100);

            builder.Entity<Address>()
                .Property(a => a.DetailAddress)
                .HasMaxLength(500)
                .IsRequired();

            builder.Entity<Address>()
                .HasIndex(a => a.UserId);

            builder.Entity<Address>()
                .HasIndex(a => new { a.UserId, a.IsDefault });

            builder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InventoryTransaction>().ToTable("InventoryTransactions");

            builder.Entity<InventoryTransaction>()
                .HasKey(it => it.InventoryTransactionId);

            builder.Entity<InventoryTransaction>()
                .Property(it => it.Type)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<InventoryTransaction>()
                .Property(it => it.Note)
                .HasMaxLength(500);

            builder.Entity<InventoryTransaction>()
                .Property(it => it.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<InventoryTransaction>()
                .HasIndex(it => it.ProductVariantId);

            builder.Entity<InventoryTransaction>()
                .HasIndex(it => it.CreatedAt);

            builder.Entity<InventoryTransaction>()
                .HasOne(it => it.ProductVariant)
                .WithMany()
                .HasForeignKey(it => it.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}