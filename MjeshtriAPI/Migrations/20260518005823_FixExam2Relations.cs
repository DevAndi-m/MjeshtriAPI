using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MjeshtriAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixExam2Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Satellites_Planets2_Planet2Id",
                table: "Satellites");

            migrationBuilder.DropForeignKey(
                name: "FK_Satellites2_Planets_PlanetId",
                table: "Satellites2");

            migrationBuilder.DropIndex(
                name: "IX_Satellites_Planet2Id",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "Planet2Id",
                table: "Satellites");

            migrationBuilder.AddForeignKey(
                name: "FK_Satellites2_Planets2_PlanetId",
                table: "Satellites2",
                column: "PlanetId",
                principalTable: "Planets2",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Satellites2_Planets2_PlanetId",
                table: "Satellites2");

            migrationBuilder.AddColumn<int>(
                name: "Planet2Id",
                table: "Satellites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Satellites_Planet2Id",
                table: "Satellites",
                column: "Planet2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Satellites_Planets2_Planet2Id",
                table: "Satellites",
                column: "Planet2Id",
                principalTable: "Planets2",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Satellites2_Planets_PlanetId",
                table: "Satellites2",
                column: "PlanetId",
                principalTable: "Planets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
