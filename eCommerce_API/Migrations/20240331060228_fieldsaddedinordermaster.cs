using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerce_API.Migrations
{
    public partial class fieldsaddedinordermaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrderMaster",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "OrderMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "OrderMaster");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "OrderMaster");
        }
    }
}
