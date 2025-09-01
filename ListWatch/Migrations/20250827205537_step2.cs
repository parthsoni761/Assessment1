using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListWatch.Migrations
{
    /// <inheritdoc />
    public partial class step2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatchProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WatchListItemId = table.Column<int>(type: "int", nullable: false),
                    CompletedEpisodes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalEpisodes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchProgress_WatchListItems_WatchListItemId",
                        column: x => x.WatchListItemId,
                        principalTable: "WatchListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchProgress_WatchListItemId",
                table: "WatchProgress",
                column: "WatchListItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchProgress");
        }
    }
}
