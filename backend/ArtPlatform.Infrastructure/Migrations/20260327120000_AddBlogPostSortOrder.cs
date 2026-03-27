using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddBlogPostSortOrder : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "SortOrder",
            table: "BlogPosts",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "SortOrder",
            table: "BlogPosts");
    }
}
