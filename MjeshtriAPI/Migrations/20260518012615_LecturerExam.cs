using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MjeshtriAPI.Migrations
{
    /// <inheritdoc />
    public partial class LecturerExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lecturers3",
                columns: table => new
                {
                    LecturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LecturerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers3", x => x.LecturerId);
                });

            migrationBuilder.CreateTable(
                name: "Lectures3",
                columns: table => new
                {
                    LectureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LectureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LecturerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lectures3", x => x.LectureId);
                    table.ForeignKey(
                        name: "FK_Lectures3_Lecturers3_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturers3",
                        principalColumn: "LecturerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lectures3_LecturerId",
                table: "Lectures3",
                column: "LecturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lectures3");

            migrationBuilder.DropTable(
                name: "Lecturers3");
        }
    }
}
