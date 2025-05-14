using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bai1.Migrations
{
    public partial class AddStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Tạo bảng Stores
            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 2. Tạo index cho OwnerId trong Stores
            migrationBuilder.CreateIndex(
                name: "IX_Stores_OwnerId",
                table: "Stores",
                column: "OwnerId");

            // 3. Thêm cột StoreId (nullable) vào Foods
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Foods",
                type: "int",
                nullable: true); // để tránh lỗi khi chưa có dữ liệu liên kết

            // 4. Tạo index cho StoreId trong Foods
            migrationBuilder.CreateIndex(
                name: "IX_Foods_StoreId",
                table: "Foods",
                column: "StoreId");

            // 5. Tạo khóa ngoại từ Foods.StoreId tới Stores.Id với SetNull khi xóa
            migrationBuilder.AddForeignKey(
                name: "FK_Foods_Stores_StoreId",
                table: "Foods",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull); // an toàn, không conflict cascade
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa foreign key từ Foods.StoreId
            migrationBuilder.DropForeignKey(
                name: "FK_Foods_Stores_StoreId",
                table: "Foods");

            // Xóa index của StoreId trong Foods
            migrationBuilder.DropIndex(
                name: "IX_Foods_StoreId",
                table: "Foods");

            // Xóa cột StoreId trong Foods
            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Foods");

            // Xóa bảng Stores
            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
