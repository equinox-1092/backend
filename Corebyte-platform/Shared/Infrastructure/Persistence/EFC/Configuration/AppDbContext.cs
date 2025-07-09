// Using IAM User instead of authentication User
using Corebyte_platform.authentication.Domain.Model.Aggregates;
using Corebyte_platform.batch_management.Domain.Model.Aggregates;
using Corebyte_platform.history_status.Domain.Model.Aggregates;
using Corebyte_platform.history_status.Domain.Model.ValueObjects;
using Corebyte_platform.IAM.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Corebyte_platform.orders.Domain.Model.Aggregates;
using Corebyte_platform.Shared.Infrastructure.Persistence.EFC.Converters;
using Corebyte_platform.Shared.Infrastucture.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Corebyte_platform.Shared.Infrastucture.Persistence.EFC.Configuration
{
    /// <summary>
    ///     Application database context
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Using IAM User instead of authentication User
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // Add the created and updated interceptor
            builder.AddCreatedUpdatedInterceptor();
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure IAM User entity
            builder.Entity<Corebyte_platform.IAM.Domain.Model.Aggregates.User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.HasIndex(u => u.Username).IsUnique();
            });
            // Configuration of the entity History
            builder.Entity<History>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Id).IsRequired().ValueGeneratedOnAdd();
                entity.Property(h => h.customer).IsRequired().HasMaxLength(100);
                entity.Property(h => h.date).IsRequired();
                entity.Property(h => h.product).IsRequired().HasMaxLength(100);
                entity.Property(h => h.amount).IsRequired().HasColumnType("int");
                entity.Property(h => h.total).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(h => h.status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<StatusValueConverter>();
            });

            //Configuration of the entity Record
            builder.Entity<Record>().HasKey(r => r.Id);
            builder.Entity<Record>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Record>().Property(r => r.customer_id).IsRequired();
            builder.Entity<Record>().Property(r => r.type).IsRequired().HasMaxLength(100);
            builder.Entity<Record>().Property(r => r.year).IsRequired();
            builder.Entity<Record>().Property(r => r.product).IsRequired().HasMaxLength(100);
            builder.Entity<Record>().Property(r => r.batch).IsRequired().HasColumnType("int");
            builder.Entity<Record>().Property(r => r.stock).IsRequired().HasColumnType("int(3)");
            
            // Configuration of the Order entity
            builder.Entity<Order>().HasKey(o => o.Id);
            builder.Entity<Order>().Property(o => o.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Order>().Property(o => o.Customer).IsRequired().HasMaxLength(100);
            builder.Entity<Order>().Property(o => o.Date).IsRequired();
            builder.Entity<Order>().Property(o => o.Product).IsRequired().HasMaxLength(100);
            builder.Entity<Order>().Property(o => o.Amount).IsRequired();
            builder.Entity<Order>().Property(o => o.Total).IsRequired().HasColumnType("decimal(18,2)");

            //Configuration of the entity Batch
            builder.Entity<Batch>().HasKey(b => b.Id);
            builder.Entity<Batch>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Batch>().Property(b => b.Name).IsRequired();
            builder.Entity<Batch>().Property(b => b.Type).IsRequired().HasMaxLength(100);
            builder.Entity<Batch>().Property(b => b.Status).IsRequired();
            builder.Entity<Batch>().Property(b => b.Temperature).IsRequired();
            builder.Entity<Batch>().Property(b => b.Amount).IsRequired().HasMaxLength(50);
            builder.Entity<Batch>().Property(b => b.Total).IsRequired().HasColumnType("decimal(18,2)");
            builder.Entity<Batch>().Property(b => b.Date).IsRequired();
            builder.Entity<Batch>().Property(b => b.NLote).IsRequired().HasMaxLength(100);
            
            //Configuration of the entity Replenishment
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().HasKey(b=> b.Id);
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.Id).ValueGeneratedOnAdd();
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.OrderNumber);
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.Name).IsRequired().HasMaxLength(100);
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.Type).IsRequired().HasMaxLength(50);
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.Date).IsRequired();
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.StockActual).IsRequired();
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.StockMinimo).IsRequired();
            builder.Entity<replenishment.Domain.Model.Aggregate.Replenishment>().Property(b => b.Price).IsRequired().HasColumnType("decimal(18,2)");
            
            // Configuration for authentication User entity
            builder.Entity<Corebyte_platform.authentication.Domain.Model.Aggregates.User>(entity =>
            {
                entity.ToTable("auth_users"); // Different table name for authentication users
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
    
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);
        
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);
        
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);
            });
            
            // Configuration for IAM User entity will be handled by ApplyIamConfiguration()

            // Apply configurations for the IAM bounded context
            builder.ApplyIamConfiguration();

            builder.UseSnakeCaseNamingConvention();
        }
    }
}
