using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FMS.Migrations
{
    /// <inheritdoc />
    public partial class AddMeterReadingsAndLedgerCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LedgerCode",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AttendantId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "LedgerCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeterReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadingValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TakenBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LedgerCodes",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "Description", "FuelType", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Fuel", "DIESEL001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diesel - Service Trucks", "Diesel", true, null },
                    { 2, "Fuel", "DIESEL002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diesel - Fuel Trailers", "Diesel", true, null },
                    { 3, "Fuel", "DIESEL003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diesel - Underground Tanks", "Diesel", true, null },
                    { 4, "Fuel", "PETROL001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Petrol - Service Vehicles", "Petrol", true, null },
                    { 5, "Fuel", "PETROL002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Petrol - Underground Tanks", "Petrol", true, null },
                    { 6, "Sundries", "SUNDRY001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sundries - Lubricants", "", true, null },
                    { 7, "Sundries", "SUNDRY002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sundries - Maintenance", "", true, null },
                    { 8, "Sundries", "SUNDRY003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sundries - Cleaning", "", true, null },
                    { 9, "Contract", "CONTRACT01", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Amplant Contract", "", true, null },
                    { 10, "Contract", "CONTRACT02", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Musina Contract", "", true, null }
                });

            migrationBuilder.InsertData(
                table: "MeterReadings",
                columns: new[] { "Id", "CreatedAt", "Notes", "PlantNumber", "ReadingDate", "ReadingType", "ReadingValue", "TakenBy", "Unit", "VehicleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "FSH01-01", new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Initial", 5000m, "System", "Kilometers", "" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "FSH02-01", new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Initial", 7500m, "System", "Kilometers", "" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "FSH03-01", new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Initial", 3000m, "System", "Kilometers", "" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "FSH04-01", new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Initial", 6200m, "System", "Kilometers", "" }
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "Capacity", "Category", "CurrentStock", "FuelType", "LastUpdated", "LastUpdatedBy", "Location", "PlantId", "PlantName" },
                values: new object[,]
                {
                    { 1, 2000m, "service_trucks", 1500m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Main Yard", "FSH01-01", "FSH01 - 01" },
                    { 2, 2000m, "service_trucks", 1800m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Main Yard", "FSH02-01", "FSH02 - 01" },
                    { 3, 2000m, "service_trucks", 1200m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Main Yard", "FSH03-01", "FSH03 - 01" },
                    { 4, 2000m, "service_trucks", 1900m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Main Yard", "FSH04-01", "FSH04 - 01" },
                    { 5, 10000m, "fuel_trailers", 5000m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Trailer Park", "SLD02", "SLD 02" },
                    { 6, 10000m, "fuel_trailers", 7500m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Trailer Park", "SLD07", "SLD 07" },
                    { 7, 10000m, "fuel_trailers", 3000m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Trailer Park", "SLD09", "SLD 09" },
                    { 8, 50000m, "underground_tanks", 15000m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Underground Storage", "UDT49", "UDT 49" },
                    { 9, 50000m, "underground_tanks", 25000m, "Petrol", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Underground Storage", "UPT49", "UPT 49" },
                    { 10, 50000m, "underground_tanks", 18000m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Underground Storage", "UDT890", "UDT 890" },
                    { 11, 50000m, "underground_tanks", 22000m, "Diesel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Underground Storage", "STD02", "STD 02" },
                    { 12, 50000m, "underground_tanks", 19000m, "Petrol", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Underground Storage", "STD05", "STD 05" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Role", "Station" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@fms.com", "Admin User", true, "$2a$11$CP/KMnsNmEog8GNUe9n8ieG8H8l0YP.RpTWUj.hcmHmrLpgz1c1o.", "admin", "Main Depot" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "attendant@fms.com", "Attendant User", true, "$2a$11$P96igEL9YWv8Zt7dU3PGjO3jNU23.RNOo5yncfnGz3/koRVaik9M6", "attendant", "Main Depot" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AttendantId",
                table: "Transactions",
                column: "AttendantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LedgerCode",
                table: "Transactions",
                column: "LedgerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerCodes_Category",
                table: "LedgerCodes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerCodes_Code",
                table: "LedgerCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerCodes_IsActive",
                table: "LedgerCodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_PlantNumber",
                table: "MeterReadings",
                column: "PlantNumber");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_PlantNumber_ReadingDate",
                table: "MeterReadings",
                columns: new[] { "PlantNumber", "ReadingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_ReadingDate",
                table: "MeterReadings",
                column: "ReadingDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LedgerCodes");

            migrationBuilder.DropTable(
                name: "MeterReadings");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AttendantId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LedgerCode",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "LedgerCode",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "AttendantId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
