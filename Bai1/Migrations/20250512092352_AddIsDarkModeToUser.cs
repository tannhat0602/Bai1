﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bai1.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDarkModeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDarkMode",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDarkMode",
                table: "AspNetUsers");
        }
    }
}
