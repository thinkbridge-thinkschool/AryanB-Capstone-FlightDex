using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDex.Flights.Infrastructure.Persistence.Migrations
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Airport = table.Column<string>(type: "varchar(3)", nullable: false),
                    Direction = table.Column<string>(type: "varchar(16)", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flights");
        }
    }
}
