using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bai1.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePartnerRequestStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerRequests_AspNetUsers_UserId",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "BusinessAddress",
                table: "PartnerRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PartnerRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PartnerRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "BusinessType",
                table: "PartnerRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "PartnerRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "PartnerRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "PartnerRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "PartnerRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerRequests_AspNetUsers_UserId",
                table: "PartnerRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerRequests_AspNetUsers_UserId",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "City",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "District",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "PartnerRequests");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "PartnerRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PartnerRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PartnerRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessAddress",
                table: "PartnerRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerRequests_AspNetUsers_UserId",
                table: "PartnerRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
