using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LocalizationManager.PostgreSql.Migrations;

/// <inheritdoc />
public partial class Initial : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "Applications",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                DefaultCulture = table.Column<string>(type: "text", nullable: false),
                AvailableCultures = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Applications", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Images",
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Label = table.Column<string>(type: "text", nullable: false),
                Bytes = table.Column<byte[]>(type: "bytea", nullable: false),
                ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                Culture = table.Column<string>(type: "text", nullable: false),
                Key = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => {
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
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                Culture = table.Column<string>(type: "text", nullable: false),
                Key = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => {
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
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Value = table.Column<string>(type: "text", nullable: false),
                ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                Culture = table.Column<string>(type: "text", nullable: false),
                Key = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Texts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Texts_Applications_ApplicationId",
                    column: x => x.ApplicationId,
                    principalTable: "Applications",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ListItems",
            columns: table => new {
                ListId = table.Column<int>(type: "integer", nullable: false),
                Index = table.Column<int>(type: "integer", nullable: false),
                Value = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_ListItems", x => new { x.ListId, x.Index });
                table.ForeignKey(
                    name: "FK_ListItems_Lists_ListId",
                    column: x => x.ListId,
                    principalTable: "Lists",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Images_ApplicationId_Culture_Key",
            table: "Images",
            columns: new[] { "ApplicationId", "Culture", "Key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ListItems_ListId_Value",
            table: "ListItems",
            columns: new[] { "ListId", "Value" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Lists_ApplicationId_Culture_Key",
            table: "Lists",
            columns: new[] { "ApplicationId", "Culture", "Key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Texts_ApplicationId_Culture_Key",
            table: "Texts",
            columns: new[] { "ApplicationId", "Culture", "Key" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "Images");

        migrationBuilder.DropTable(
            name: "ListItems");

        migrationBuilder.DropTable(
            name: "Texts");

        migrationBuilder.DropTable(
            name: "Lists");

        migrationBuilder.DropTable(
            name: "Applications");
    }
}
