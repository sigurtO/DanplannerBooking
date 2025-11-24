using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanplannerBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bundles_BundleId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleAddOn_Bundles_BundleId",
                table: "BundleAddOn");

            migrationBuilder.DropForeignKey(
                name: "FK_Bundles_Cottages_CottageId",
                table: "Bundles");

            migrationBuilder.DropForeignKey(
                name: "FK_Bundles_Spaces_SpaceId",
                table: "Bundles");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BundleId",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bundles",
                table: "Bundles");

            migrationBuilder.DropColumn(
                name: "BundleId",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "Bundles",
                newName: "Bundle");

            migrationBuilder.RenameIndex(
                name: "IX_Bundles_SpaceId",
                table: "Bundle",
                newName: "IX_Bundle_SpaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bundles_CottageId",
                table: "Bundle",
                newName: "IX_Bundle_CottageId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Spaces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Spaces",
                type: "nvarchar(max)",
                nullable: true);

            // ImageUrl på Campsites findes allerede i databasen,
            // så vi lader være med at tilføje den igen her.

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bundle",
                table: "Bundle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bundle_Cottages_CottageId",
                table: "Bundle",
                column: "CottageId",
                principalTable: "Cottages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bundle_Spaces_SpaceId",
                table: "Bundle",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleAddOn_Bundle_BundleId",
                table: "BundleAddOn",
                column: "BundleId",
                principalTable: "Bundle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bundle_Cottages_CottageId",
                table: "Bundle");

            migrationBuilder.DropForeignKey(
                name: "FK_Bundle_Spaces_SpaceId",
                table: "Bundle");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleAddOn_Bundle_BundleId",
                table: "BundleAddOn");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bundle",
                table: "Bundle");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Spaces");

            // Her droppede vi før ImageUrl-kolonnen, men da vi ikke længere
            // tilføjer den i Up, skal vi heller ikke fjerne den i Down.

            migrationBuilder.RenameTable(
                name: "Bundle",
                newName: "Bundles");

            migrationBuilder.RenameIndex(
                name: "IX_Bundle_SpaceId",
                table: "Bundles",
                newName: "IX_Bundles_SpaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bundle_CottageId",
                table: "Bundles",
                newName: "IX_Bundles_CottageId");

            migrationBuilder.AddColumn<Guid>(
                name: "BundleId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bundles",
                table: "Bundles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BundleId",
                table: "Bookings",
                column: "BundleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bundles_BundleId",
                table: "Bookings",
                column: "BundleId",
                principalTable: "Bundles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleAddOn_Bundles_BundleId",
                table: "BundleAddOn",
                column: "BundleId",
                principalTable: "Bundles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bundles_Cottages_CottageId",
                table: "Bundles",
                column: "CottageId",
                principalTable: "Cottages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bundles_Spaces_SpaceId",
                table: "Bundles",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");
        }
    }
}
