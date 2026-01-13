using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Betly.data.Migrations
{
    /// <inheritdoc />
    public partial class RestoreEventInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Table already exists from a previously applied migration that was lost in rollback.
            // Skipping creation to avoid error.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInvitations");
        }
    }
}
