using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace sharedfile.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessToken",
                columns: table => new
                {
                    GUID = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    TokenCreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToken", x => x.GUID);
                });

            migrationBuilder.CreateTable(
                name: "FileManagement",
                columns: table => new
                {
                    GUID = table.Column<string>(nullable: false),
                    FileAreaGUID = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    BlobUrl = table.Column<string>(nullable: true),
                    SanitizedToken = table.Column<string>(nullable: true),
                    UploadedDate = table.Column<string>(nullable: true),
                    FileSize = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileManagement", x => x.GUID);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    GUID = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Division = table.Column<string>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    FileAreaGUID = table.Column<string>(nullable: true),
                    FileGUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.GUID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessToken");

            migrationBuilder.DropTable(
                name: "FileManagement");

            migrationBuilder.DropTable(
                name: "Log");
        }
    }
}
