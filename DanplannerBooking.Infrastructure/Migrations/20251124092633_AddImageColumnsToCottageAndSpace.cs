using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanplannerBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageColumnsToCottageAndSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Cottages");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Bookings");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Spaces",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Cottages",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Cottages");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Spaces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Cottages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
