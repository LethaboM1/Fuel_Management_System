using Microsoft.EntityFrameworkCore;
using FMS.Models;

namespace FMS.Data
{
    public class FMSDbContext : DbContext
    {
        public FMSDbContext(DbContextOptions<FMSDbContext> options) : base(options)
        {
        }

        public DbSet<Transactions> Transactions => Set<Transactions>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<User> Users => Set<User>();
        public DbSet<LedgerCode> LedgerCodes => Set<LedgerCode>();
        public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure indexes
            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasIndex(t => t.PlantNumber);
                entity.HasIndex(t => t.TransactionDate);
                entity.HasIndex(t => t.AttendantId);
                entity.HasIndex(t => t.LedgerCode);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasIndex(s => s.PlantId).IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<LedgerCode>(entity =>
            {
                entity.HasIndex(l => l.Code).IsUnique();
                entity.HasIndex(l => l.Category);
                entity.HasIndex(l => l.IsActive);
            });

            modelBuilder.Entity<MeterReading>(entity =>
            {
                entity.HasIndex(m => m.PlantNumber);
                entity.HasIndex(m => m.ReadingDate);
                entity.HasIndex(m => new { m.PlantNumber, m.ReadingDate });
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed initial users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin User",
                    Email = "admin@fms.com",
                    Role = "admin",
                    Station = "Main Depot",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedAt = seedDate,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    FullName = "Attendant User",
                    Email = "attendant@fms.com",
                    Role = "attendant",
                    Station = "Main Depot",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("attendant123"),
                    CreatedAt = seedDate,
                    IsActive = true
                }
            );

            // Seed ledger codes
            modelBuilder.Entity<LedgerCode>().HasData(
                // Diesel Codes
                new LedgerCode { Id = 1, Code = "DIESEL001", Description = "Diesel - Service Trucks", Category = "Fuel", FuelType = "Diesel", IsActive = true, CreatedAt = seedDate },
                new LedgerCode { Id = 2, Code = "DIESEL002", Description = "Diesel - Fuel Trailers", Category = "Fuel", FuelType = "Diesel", IsActive = true, CreatedAt = seedDate },
                new LedgerCode { Id = 3, Code = "DIESEL003", Description = "Diesel - Underground Tanks", Category = "Fuel", FuelType = "Diesel", IsActive = true, CreatedAt = seedDate },
                
                // Petrol Codes
                new LedgerCode { Id = 4, Code = "PETROL001", Description = "Petrol - Service Vehicles", Category = "Fuel", FuelType = "Petrol", IsActive = true, CreatedAt = seedDate },
                new LedgerCode { Id = 5, Code = "PETROL002", Description = "Petrol - Underground Tanks", Category = "Fuel", FuelType = "Petrol", IsActive = true, CreatedAt = seedDate },
                
                // Sundries Codes
                new LedgerCode { Id = 6, Code = "SUNDRY001", Description = "Sundries - Lubricants", Category = "Sundries", FuelType = "", IsActive = true, CreatedAt = seedDate },
                new LedgerCode { Id = 7, Code = "SUNDRY002", Description = "Sundries - Maintenance", Category = "Sundries", FuelType = "", IsActive = true, CreatedAt = seedDate },
                new LedgerCode { Id = 8, Code = "SUNDRY003", Description = "Sundries - Cleaning", Category = "Sundries", FuelType = "", IsActive = true, CreatedAt = seedDate },
                
                // Contract Codes
                new LedgerCode { Id = 9, Code = "CONTRACT01", Description = "Amplant Contract", Category = "Contract", FuelType = "", IsActive = true, CreatedAt = seedDate },
                new LedgerCode { Id = 10, Code = "CONTRACT02", Description = "Musina Contract", Category = "Contract", FuelType = "", IsActive = true, CreatedAt = seedDate }
            );

            // Seed initial stock data
            modelBuilder.Entity<Stock>().HasData(
                // Service Trucks
                new Stock { Id = 1, PlantId = "FSH01-01", PlantName = "FSH01 - 01", Category = "service_trucks", CurrentStock = 1500, Capacity = 2000, FuelType = "Diesel", Location = "Main Yard", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 2, PlantId = "FSH02-01", PlantName = "FSH02 - 01", Category = "service_trucks", CurrentStock = 1800, Capacity = 2000, FuelType = "Diesel", Location = "Main Yard", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 3, PlantId = "FSH03-01", PlantName = "FSH03 - 01", Category = "service_trucks", CurrentStock = 1200, Capacity = 2000, FuelType = "Diesel", Location = "Main Yard", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 4, PlantId = "FSH04-01", PlantName = "FSH04 - 01", Category = "service_trucks", CurrentStock = 1900, Capacity = 2000, FuelType = "Diesel", Location = "Main Yard", LastUpdated = seedDate, LastUpdatedBy = "System" },
                
                // Fuel Trailers
                new Stock { Id = 5, PlantId = "SLD02", PlantName = "SLD 02", Category = "fuel_trailers", CurrentStock = 5000, Capacity = 10000, FuelType = "Diesel", Location = "Trailer Park", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 6, PlantId = "SLD07", PlantName = "SLD 07", Category = "fuel_trailers", CurrentStock = 7500, Capacity = 10000, FuelType = "Diesel", Location = "Trailer Park", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 7, PlantId = "SLD09", PlantName = "SLD 09", Category = "fuel_trailers", CurrentStock = 3000, Capacity = 10000, FuelType = "Diesel", Location = "Trailer Park", LastUpdated = seedDate, LastUpdatedBy = "System" },
                
                // Underground Tanks
                new Stock { Id = 8, PlantId = "UDT49", PlantName = "UDT 49", Category = "underground_tanks", CurrentStock = 15000, Capacity = 50000, FuelType = "Diesel", Location = "Underground Storage", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 9, PlantId = "UPT49", PlantName = "UPT 49", Category = "underground_tanks", CurrentStock = 25000, Capacity = 50000, FuelType = "Petrol", Location = "Underground Storage", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 10, PlantId = "UDT890", PlantName = "UDT 890", Category = "underground_tanks", CurrentStock = 18000, Capacity = 50000, FuelType = "Diesel", Location = "Underground Storage", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 11, PlantId = "STD02", PlantName = "STD 02", Category = "underground_tanks", CurrentStock = 22000, Capacity = 50000, FuelType = "Diesel", Location = "Underground Storage", LastUpdated = seedDate, LastUpdatedBy = "System" },
                new Stock { Id = 12, PlantId = "STD05", PlantName = "STD 05", Category = "underground_tanks", CurrentStock = 19000, Capacity = 50000, FuelType = "Petrol", Location = "Underground Storage", LastUpdated = seedDate, LastUpdatedBy = "System" }
            );

            // Seed initial meter readings
            modelBuilder.Entity<MeterReading>().HasData(
                new MeterReading { Id = 1, PlantNumber = "FSH01-01", ReadingDate = seedDate.AddDays(-1), ReadingValue = 5000, Unit = "Kilometers", ReadingType = "Initial", TakenBy = "System", CreatedAt = seedDate },
                new MeterReading { Id = 2, PlantNumber = "FSH02-01", ReadingDate = seedDate.AddDays(-1), ReadingValue = 7500, Unit = "Kilometers", ReadingType = "Initial", TakenBy = "System", CreatedAt = seedDate },
                new MeterReading { Id = 3, PlantNumber = "FSH03-01", ReadingDate = seedDate.AddDays(-1), ReadingValue = 3000, Unit = "Kilometers", ReadingType = "Initial", TakenBy = "System", CreatedAt = seedDate },
                new MeterReading { Id = 4, PlantNumber = "FSH04-01", ReadingDate = seedDate.AddDays(-1), ReadingValue = 6200, Unit = "Kilometers", ReadingType = "Initial", TakenBy = "System", CreatedAt = seedDate }
            );
        }
    }
}