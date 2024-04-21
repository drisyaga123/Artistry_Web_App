using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerce_API.Migrations
{
    public partial class otpcolumnaddedinordermaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "OrderMaster",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTP",
                table: "OrderMaster");
        }
    }
}
