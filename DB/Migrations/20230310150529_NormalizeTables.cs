using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birth_day",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "card_cvc",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "card_exp",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "card_number",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "card_type",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "city",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "email_address",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "state",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "street_address",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "zip_code",
                table: "reservation");

            migrationBuilder.AddColumn<int>(
                name: "cardId",
                table: "reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "guestSSN",
                table: "reservation",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "guest",
                columns: table => new
                {
                    SSN = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    birth_day = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    street_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    zip_code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guest", x => x.SSN);
                });

            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    card_type = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    guestSSN = table.Column<long>(type: "bigint", nullable: false),
                    card_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    card_exp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    card_cvc = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cards_guest_guestSSN",
                        column: x => x.guestSSN,
                        principalTable: "guest",
                        principalColumn: "SSN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservation_cardId",
                table: "reservation",
                column: "cardId");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_guestSSN",
                table: "reservation",
                column: "guestSSN");

            migrationBuilder.CreateIndex(
                name: "IX_cards_guestSSN",
                table: "cards",
                column: "guestSSN");

            migrationBuilder.AddForeignKey(
                name: "FK_reservation_cards_cardId",
                table: "reservation",
                column: "cardId",
                principalTable: "cards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservation_guest_guestSSN",
                table: "reservation",
                column: "guestSSN",
                principalTable: "guest",
                principalColumn: "SSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservation_cards_cardId",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_reservation_guest_guestSSN",
                table: "reservation");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "guest");

            migrationBuilder.DropIndex(
                name: "IX_reservation_cardId",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "IX_reservation_guestSSN",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "cardId",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "guestSSN",
                table: "reservation");

            migrationBuilder.AddColumn<string>(
                name: "birth_day",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "card_cvc",
                table: "reservation",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "card_exp",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "card_number",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "card_type",
                table: "reservation",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "reservation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email_address",
                table: "reservation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "reservation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "street_address",
                table: "reservation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "zip_code",
                table: "reservation",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
