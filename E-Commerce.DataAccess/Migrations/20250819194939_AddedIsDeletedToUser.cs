using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDeletedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "ApplicationUsers",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ApplicationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ApplicationUsers");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ApplicationUsers",
                newName: "CreateAt");
        }
    }
}
