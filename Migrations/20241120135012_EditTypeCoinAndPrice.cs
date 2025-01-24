using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnerDuo.Migrations
{
    /// <inheritdoc />
    public partial class EditTypeCoinAndPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Coin",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Users",
                type: "DECIMAL(10,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Coin",
                table: "Users",
                type: "DECIMAL(10,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
