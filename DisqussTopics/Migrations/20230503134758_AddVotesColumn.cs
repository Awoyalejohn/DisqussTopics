using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisqussTopics.Migrations
{
    /// <inheritdoc />
    public partial class AddVotesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Votes",
                table: "Posts");
        }
    }
}
