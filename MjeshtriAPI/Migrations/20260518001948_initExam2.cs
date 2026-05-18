using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MjeshtriAPI.Migrations
{
    /// <inheritdoc />
    public partial class initExam2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Planet2Id",
                table: "Satellites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Planets2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planets2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Satellites2",
                columns: table => new
                {
                    SatelliteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Satellites2", x => x.SatelliteId);
                    table.ForeignKey(
                        name: "FK_Satellites2_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Satellites_Planet2Id",
                table: "Satellites",
                column: "Planet2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Satellites2_PlanetId",
                table: "Satellites2",
                column: "PlanetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Satellites_Planets2_Planet2Id",
                table: "Satellites",
                column: "Planet2Id",
                principalTable: "Planets2",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Satellites_Planets2_Planet2Id",
                table: "Satellites");

            migrationBuilder.DropTable(
                name: "Planets2");

            migrationBuilder.DropTable(
                name: "Satellites2");

            migrationBuilder.DropIndex(
                name: "IX_Satellites_Planet2Id",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "Planet2Id",
                table: "Satellites");
        }
    }
}
