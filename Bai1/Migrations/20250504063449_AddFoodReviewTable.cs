using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bai1.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DiscountCodes");

            migrationBuilder.RenameColumn(
                name: "UsageCount",
                table: "DiscountCodes",
                newName: "StoreId");

            migrationBuilder.AlterColumn<int>(
                name: "MaxUsageCount",
                table: "DiscountCodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "DiscountCodes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DiscountCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "DiscountCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "FoodReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    PartnerReply = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodReviews_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCodes_StoreId",
                table: "DiscountCodes",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReviews_FoodId",
                table: "FoodReviews",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReviews_UserId",
                table: "FoodReviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCodes_Stores_StoreId",
                table: "DiscountCodes",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCodes_Stores_StoreId",
                table: "DiscountCodes");

            migrationBuilder.DropTable(
                name: "FoodReviews");

            migrationBuilder.DropIndex(
                name: "IX_DiscountCodes_StoreId",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "DiscountCodes");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "DiscountCodes",
                newName: "UsageCount");

            migrationBuilder.AlterColumn<int>(
                name: "MaxUsageCount",
                table: "DiscountCodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "DiscountCodes",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "DiscountPercent",
                table: "DiscountCodes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "DiscountCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DiscountCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
