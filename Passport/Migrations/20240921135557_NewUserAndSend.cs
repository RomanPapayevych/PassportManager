using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passport.Migrations
{
    /// <inheritdoc />
    public partial class NewUserAndSend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SendData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SendData_UserId",
                table: "SendData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrderId",
                table: "AspNetUsers",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SendData_OrderId",
                table: "AspNetUsers",
                column: "OrderId",
                principalTable: "SendData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SendData_AspNetUsers_UserId",
                table: "SendData",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SendData_OrderId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SendData_AspNetUsers_UserId",
                table: "SendData");

            migrationBuilder.DropIndex(
                name: "IX_SendData_UserId",
                table: "SendData");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrderId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SendData");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "AspNetUsers");
        }
    }
}
