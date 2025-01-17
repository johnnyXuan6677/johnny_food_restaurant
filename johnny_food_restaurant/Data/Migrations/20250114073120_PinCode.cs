﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace johnny_food_restaurant.Data.Migrations
{
    public partial class PinCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "AspNetUsers");
        }
    }
}
