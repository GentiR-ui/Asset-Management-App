using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace AssetManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoveringIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_Category",
                table: "Assets");
            migrationBuilder.DropIndex(
                name: "IX_Assets_DepartmentId",
                table: "Assets");
            migrationBuilder.DropIndex(
                name: "IX_Assets_Status",
                table: "Assets");
            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAtUtc",
                table: "RefreshTokens",
                column: "ExpiresAtUtc");
            migrationBuilder.CreateIndex(
                name: "IX_Assets_Category_Price",
                table: "Assets",
                column: "Category")
                .Annotation("SqlServer:Include", new[] { "PurchasePrice", "DepartmentId" });
            migrationBuilder.CreateIndex(
                name: "IX_Assets_Dept_Tag_Price",
                table: "Assets",
                columns: new[] { "DepartmentId", "AssetTag" })
                .Annotation("SqlServer:Include", new[] { "PurchasePrice" });
            migrationBuilder.CreateIndex(
                name: "IX_Assets_PurchaseDate_Price",
                table: "Assets",
                column: "PurchaseDate")
                .Annotation("SqlServer:Include", new[] { "PurchasePrice", "DepartmentId" });
            migrationBuilder.CreateIndex(
                name: "IX_Assets_Status_Price",
                table: "Assets",
                column: "Status")
                .Annotation("SqlServer:Include", new[] { "PurchasePrice", "DepartmentId" });
            migrationBuilder.CreateIndex(
                name: "IX_Assets_Warranty",
                table: "Assets",
                column: "WarrantyExpiryDate")
                .Annotation("SqlServer:Include", new[] { "AssetTag", "Name", "DepartmentId" });
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ExpiresAtUtc",
                table: "RefreshTokens");
            migrationBuilder.DropIndex(
                name: "IX_Assets_Category_Price",
                table: "Assets");
            migrationBuilder.DropIndex(
                name: "IX_Assets_Dept_Tag_Price",
                table: "Assets");
            migrationBuilder.DropIndex(
                name: "IX_Assets_PurchaseDate_Price",
                table: "Assets");
            migrationBuilder.DropIndex(
                name: "IX_Assets_Status_Price",
                table: "Assets");
            migrationBuilder.DropIndex(
                name: "IX_Assets_Warranty",
                table: "Assets");
            migrationBuilder.CreateIndex(
                name: "IX_Assets_Category",
                table: "Assets",
                column: "Category");
            migrationBuilder.CreateIndex(
                name: "IX_Assets_DepartmentId",
                table: "Assets",
                column: "DepartmentId");
            migrationBuilder.CreateIndex(
                name: "IX_Assets_Status",
                table: "Assets",
                column: "Status");
        }
    }
}
