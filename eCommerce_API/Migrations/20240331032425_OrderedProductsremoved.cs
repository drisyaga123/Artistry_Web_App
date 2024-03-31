using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerce_API.Migrations
{
    public partial class OrderedProductsremoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderedProducts");

            migrationBuilder.AddColumn<decimal>(
                name: "MRPAmount",
                table: "OrderMaster",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "OrderMaster",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "OrderMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderMaster",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SellerAddress",
                table: "OrderMaster",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerName",
                table: "OrderMaster",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SellingAmount",
                table: "OrderMaster",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MRPAmount",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "SellerAddress",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "SellerName",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "SellingAmount",
                table: "OrderMaster");

            migrationBuilder.CreateTable(
                name: "OrderedProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MRPAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SellerAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SellerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SellingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedProducts", x => x.Id);
                });
        }
    }
}
