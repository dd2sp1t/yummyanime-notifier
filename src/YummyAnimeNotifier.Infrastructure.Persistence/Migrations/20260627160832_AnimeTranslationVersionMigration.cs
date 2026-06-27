using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YummyAnimeNotifier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AnimeTranslationVersionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AnimeTranslation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "AnimeTranslation");
        }
    }
}
