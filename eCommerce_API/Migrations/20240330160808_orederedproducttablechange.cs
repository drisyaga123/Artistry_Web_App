using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerce_API.Migrations
{
    public partial class orederedproducttablechange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderedProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderedProducts");
        }
    }
}
