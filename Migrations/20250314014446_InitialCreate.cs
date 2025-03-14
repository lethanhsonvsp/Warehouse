using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Location_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Parent_Location_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Location_ID);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_Parent_Location_ID",
                        column: x => x.Parent_Location_ID,
                        principalTable: "Locations",
                        principalColumn: "Location_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Robots",
                columns: table => new
                {
                    Robot_ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Robots", x => x.Robot_ID);
                });

            migrationBuilder.CreateTable(
                name: "Pallets",
                columns: table => new
                {
                    Pallet_ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Creation_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Robot_ID = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pallets", x => x.Pallet_ID);
                    table.ForeignKey(
                        name: "FK_Pallets_Robots_Robot_ID",
                        column: x => x.Robot_ID,
                        principalTable: "Robots",
                        principalColumn: "Robot_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pallet_Locations",
                columns: table => new
                {
                    Pallet_ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Time_In = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Time_Out = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pallet_Locations", x => new { x.Pallet_ID, x.Time_In });
                    table.ForeignKey(
                        name: "FK_Pallet_Locations_Locations_Location_ID",
                        column: x => x.Location_ID,
                        principalTable: "Locations",
                        principalColumn: "Location_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pallet_Locations_Pallets_Pallet_ID",
                        column: x => x.Pallet_ID,
                        principalTable: "Pallets",
                        principalColumn: "Pallet_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Parent_Location_ID",
                table: "Locations",
                column: "Parent_Location_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pallet_Locations_Location_ID",
                table: "Pallet_Locations",
                column: "Location_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Robot_ID",
                table: "Pallets",
                column: "Robot_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pallet_Locations");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Pallets");

            migrationBuilder.DropTable(
                name: "Robots");
        }
    }
}
