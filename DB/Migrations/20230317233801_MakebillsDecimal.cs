using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class MakebillsDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "total_bill",
                table: "reservation",
                type: "float(2)",
                precision: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "food_bill",
                table: "reservation",
                type: "float(2)",
                precision: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "total_bill",
                table: "reservation",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(2)",
                oldPrecision: 2);

            migrationBuilder.AlterColumn<double>(
                name: "food_bill",
                table: "reservation",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(2)",
                oldPrecision: 2);
        }
    }
}
