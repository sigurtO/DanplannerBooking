using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanplannerBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddOns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddOns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campsites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "N/A"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    HasOceanAccess = table.Column<bool>(type: "bit", nullable: false),
                    HasPool = table.Column<bool>(type: "bit", nullable: false),
                    HasPlayground = table.Column<bool>(type: "bit", nullable: false),
                    HasCarCharger = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campsites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cottages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampsiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "N/A"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    X = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Y = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HasToilet = table.Column<bool>(type: "bit", nullable: false),
                    HasShower = table.Column<bool>(type: "bit", nullable: false),
                    HasKitchen = table.Column<bool>(type: "bit", nullable: false),
                    HasHeating = table.Column<bool>(type: "bit", nullable: false),
                    HasWiFi = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cottages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cottages_Campsites_CampsiteId",
                        column: x => x.CampsiteId,
                        principalTable: "Campsites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampsiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Y = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasElectricity = table.Column<bool>(type: "bit", nullable: false),
                    MetersFromToilet = table.Column<int>(type: "int", nullable: false),
                    MetersFromPool = table.Column<int>(type: "int", nullable: true),
                    MetersFromPlayground = table.Column<int>(type: "int", nullable: true),
                    MetersFromOcean = table.Column<int>(type: "int", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spaces_Campsites_CampsiteId",
                        column: x => x.CampsiteId,
                        principalTable: "Campsites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CottageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Cottages_CottageId",
                        column: x => x.CottageId,
                        principalTable: "Cottages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bundle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CottageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bundle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bundle_Cottages_CottageId",
                        column: x => x.CottageId,
                        principalTable: "Cottages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bundle_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookingAddOn",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAddOn", x => new { x.BookingId, x.AddOnId });
                    table.ForeignKey(
                        name: "FK_BookingAddOn_AddOns_AddOnId",
                        column: x => x.AddOnId,
                        principalTable: "AddOns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingAddOn_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BundleAddOn",
                columns: table => new
                {
                    BundleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleAddOn", x => new { x.BundleId, x.AddOnId });
                    table.ForeignKey(
                        name: "FK_BundleAddOn_AddOns_AddOnId",
                        column: x => x.AddOnId,
                        principalTable: "AddOns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BundleAddOn_Bundle_BundleId",
                        column: x => x.BundleId,
                        principalTable: "Bundle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddOn_AddOnId",
                table: "BookingAddOn",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CottageId",
                table: "Bookings",
                column: "CottageId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SpaceId",
                table: "Bookings",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bundle_CottageId",
                table: "Bundle",
                column: "CottageId");

            migrationBuilder.CreateIndex(
                name: "IX_Bundle_SpaceId",
                table: "Bundle",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_BundleAddOn_AddOnId",
                table: "BundleAddOn",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_Cottages_CampsiteId",
                table: "Cottages",
                column: "CampsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_CampsiteId",
                table: "Spaces",
                column: "CampsiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingAddOn");

            migrationBuilder.DropTable(
                name: "BundleAddOn");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "AddOns");

            migrationBuilder.DropTable(
                name: "Bundle");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cottages");

            migrationBuilder.DropTable(
                name: "Spaces");

            migrationBuilder.DropTable(
                name: "Campsites");
        }
    }
}
