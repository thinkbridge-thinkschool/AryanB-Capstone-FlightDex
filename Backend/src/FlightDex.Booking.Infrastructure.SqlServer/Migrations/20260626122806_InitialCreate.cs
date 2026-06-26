using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDex.Booking.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    OriginCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    OriginAirport = table.Column<string>(type: "varchar(128)", nullable: false),
                    OriginCity = table.Column<string>(type: "varchar(128)", nullable: false),
                    DestinationCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    DestinationAirport = table.Column<string>(type: "varchar(128)", nullable: false),
                    DestinationCity = table.Column<string>(type: "varchar(128)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(128)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(128)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(256)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(128)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(128)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    IsGovernmentOfficial = table.Column<bool>(type: "bit", nullable: false),
                    IsLawEnforcementOrMilitary = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(256)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "varchar(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
