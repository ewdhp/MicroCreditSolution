using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCredit.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "Account",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                schema: "Account",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LoanDescription",
                schema: "Account",
                table: "Loans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "Account",
                table: "Loans",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                schema: "Account",
                table: "Loans",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LoanDescription",
                schema: "Account",
                table: "Loans",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
