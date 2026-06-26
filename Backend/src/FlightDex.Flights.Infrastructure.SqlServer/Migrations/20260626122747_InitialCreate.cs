using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDex.Flights.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Airport = table.Column<string>(type: "varchar(3)", nullable: false),
                    Direction = table.Column<string>(type: "varchar(16)", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    AirlineCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    Airline = table.Column<string>(type: "varchar(128)", nullable: false),
                    CounterpartAirport = table.Column<string>(type: "varchar(128)", nullable: false),
                    CounterpartCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    CounterpartCity = table.Column<string>(type: "varchar(128)", nullable: false),
                    FlightCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    Duration = table.Column<string>(type: "varchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(8)", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: false),
                    City = table.Column<string>(type: "varchar(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_Airport_Direction_Time",
                table: "Flights",
                columns: new[] { "Airport", "Direction", "ScheduledTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_CounterpartCode",
                table: "Flights",
                column: "CounterpartCode");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_FlightCode",
                table: "Flights",
                column: "FlightCode");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
