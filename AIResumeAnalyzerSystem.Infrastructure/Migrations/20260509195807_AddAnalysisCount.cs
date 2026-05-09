using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIResumeAnalyzerSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnalysisCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalysisCount",
                table: "Users");
        }
    }
}
