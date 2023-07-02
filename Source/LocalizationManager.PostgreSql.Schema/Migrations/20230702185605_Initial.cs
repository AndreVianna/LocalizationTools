using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LocalizationProvider.PostgreSql.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     name: "Applications",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Name = table.Column<string>(type: "text", nullable: false),
                                         DefaultCulture = table.Column<string>(type: "text", nullable: false),
                                         AvailableCultures = table.Column<string>(type: "text", nullable: false)
                                     },
                                     constraints: table => table.PrimaryKey("PK_Applications", x => x.Id));

        migrationBuilder.CreateTable(
                                     name: "Images",
                                     columns: table => new
                                     {
                                         Id = table.Column<int>(type: "integer", nullable: false)
                                                   .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                         Bytes = table.Column<byte[]>(type: "bytea", nullable: false),
                                         ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                                         Culture = table.Column<string>(type: "text", nullable: false),
                                         ResourceId = table.Column<string>(type: "text", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Images", x => x.Id);
                                         table.ForeignKey(
                                                          name: "FK_Images_Applications_ApplicationId",
                                                          column: x => x.ApplicationId,
                                                          principalTable: "Applications",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Restrict);
                                     });

        migrationBuilder.CreateTable(
                                     name: "Lists",
                                     columns: table => new
                                     {
                                         Id = table.Column<int>(type: "integer", nullable: false)
                                                   .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                         ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                                         Culture = table.Column<string>(type: "text", nullable: false),
                                         ResourceId = table.Column<string>(type: "text", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Lists", x => x.Id);
                                         table.ForeignKey(
                                                          name: "FK_Lists_Applications_ApplicationId",
                                                          column: x => x.ApplicationId,
                                                          principalTable: "Applications",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Restrict);
                                     });

        migrationBuilder.CreateTable(
                                     name: "Texts",
                                     columns: table => new
                                     {
                                         Id = table.Column<int>(type: "integer", nullable: false)
                                                   .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                         Value = table.Column<string>(type: "text", nullable: false),
                                         ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                                         Culture = table.Column<string>(type: "text", nullable: false),
                                         ResourceId = table.Column<string>(type: "text", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Texts", x => x.Id);
                                         table.ForeignKey(
                                                          name: "FK_Texts_Applications_ApplicationId",
                                                          column: x => x.ApplicationId,
                                                          principalTable: "Applications",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Restrict);
                                     });

        migrationBuilder.CreateTable(
                                     name: "ListOptions",
                                     columns: table => new
                                     {
                                         ListId = table.Column<int>(type: "integer", nullable: false),
                                         OptionId = table.Column<int>(type: "integer", nullable: false),
                                         Index = table.Column<int>(type: "integer", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_ListOptions", x => new { x.ListId, x.OptionId });
                                         table.ForeignKey(
                                                          name: "FK_ListOptions_Lists_ListId",
                                                          column: x => x.ListId,
                                                          principalTable: "Lists",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Restrict);
                                         table.ForeignKey(
                                                          name: "FK_ListOptions_Texts_OptionId",
                                                          column: x => x.OptionId,
                                                          principalTable: "Texts",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Restrict);
                                     });

        migrationBuilder.CreateIndex(
                                     name: "IX_Images_ApplicationId_Culture_ResourceId",
                                     table: "Images",
                                     columns: new[] { "ApplicationId", "Culture", "ResourceId" },
                                     unique: true);

        migrationBuilder.CreateIndex(
                                     name: "IX_ListOptions_ListId_Index",
                                     table: "ListOptions",
                                     columns: new[] { "ListId", "Index" },
                                     unique: true);

        migrationBuilder.CreateIndex(
                                     name: "IX_ListOptions_OptionId",
                                     table: "ListOptions",
                                     column: "OptionId");

        migrationBuilder.CreateIndex(
                                     name: "IX_Lists_ApplicationId_Culture_ResourceId",
                                     table: "Lists",
                                     columns: new[] { "ApplicationId", "Culture", "ResourceId" },
                                     unique: true);

        migrationBuilder.CreateIndex(
                                     name: "IX_Texts_ApplicationId_Culture_ResourceId",
                                     table: "Texts",
                                     columns: new[] { "ApplicationId", "Culture", "ResourceId" },
                                     unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "Images");

        migrationBuilder.DropTable(
                                   name: "ListOptions");

        migrationBuilder.DropTable(
                                   name: "Lists");

        migrationBuilder.DropTable(
                                   name: "Texts");

        migrationBuilder.DropTable(
                                   name: "Applications");
    }
}