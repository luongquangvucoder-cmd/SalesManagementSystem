using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales_Management_System_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: ["Id", "Name", "NormalizedName", "ConcurrencyStamp"],
                values: new object[,]
                {
            { Guid.NewGuid().ToString(), "Admin",    "ADMIN",    Guid.NewGuid().ToString() },
            { Guid.NewGuid().ToString(), "Staff",    "STAFF",    Guid.NewGuid().ToString() },
            { Guid.NewGuid().ToString(), "Customer", "CUSTOMER", Guid.NewGuid().ToString() },
            { Guid.NewGuid().ToString(), "Supplier", "SUPPLIER", Guid.NewGuid().ToString() }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "NormalizedName",
                keyValues: ["ADMIN", "STAFF", "CUSTOMER", "SUPPLIER"]
            );
        }
    }
}
