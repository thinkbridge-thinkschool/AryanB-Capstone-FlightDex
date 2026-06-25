using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDex.Flights.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestructureLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_Value",
                table: "Locations");

            // The old flat rows can't be reshaped into (code, name, city) triples, and the
            // cache is fully rebuilt from the timetable at startup anyway. Clear the table so
            // the new unique Code index isn't violated by legacy rows defaulting to Code="".
            migrationBuilder.Sql("DELETE FROM \"Locations\";");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Locations",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Locations",
                type: "varchar(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Locations",
                type: "varchar(8)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_Code",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Locations");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Locations",
                newName: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Value",
                table: "Locations",
                column: "Value",
                unique: true);
        }
    }
}
