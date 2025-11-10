using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class InitDbSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guitarists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guitarists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Techniques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Techniques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tunings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tunings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuitaristId = table.Column<int>(type: "int", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Guitarists_GuitaristId",
                        column: x => x.GuitaristId,
                        principalTable: "Guitarists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechniqueId = table.Column<int>(type: "int", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Techniques_TechniqueId",
                        column: x => x.TechniqueId,
                        principalTable: "Techniques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Difficulty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BPM = table.Column<int>(type: "int", nullable: false),
                    TabNotation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TuningId = table.Column<int>(type: "int", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_Tunings_TuningId",
                        column: x => x.TuningId,
                        principalTable: "Tunings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuitaristLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuitaristId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgressPercent = table.Column<double>(type: "float", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuitaristLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuitaristLessons_Guitarists_GuitaristId",
                        column: x => x.GuitaristId,
                        principalTable: "Guitarists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuitaristLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonExercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    Asset = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonExercises_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Guitarists",
                columns: new[] { "Id", "Asset", "ExperienceYears", "IsDeleted", "Name", "SkillLevel" },
                values: new object[,]
                {
                    { 1, true, 5, false, "Aníbal Alvarado Andrade", "Advanced" },
                    { 2, true, 1, false, "Brayan Stiven Carvajal", "Intermediate" },
                    { 3, true, 1, false, "Diego Fernando Cuellar", "Beginner" }
                });

            migrationBuilder.InsertData(
                table: "Techniques",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, "Movimientos alternados de púa para mejorar velocidad y precisión.", false, "Alternate Picking" },
                    { 2, true, "Técnica centrada en hammer-ons y pull-offs para lograr fluidez.", false, "Legato" },
                    { 3, true, "Desplazamientos continuos de púa a través de cuerdas para ejecutar arpegios rápidos.", false, "Sweep Picking" }
                });

            migrationBuilder.InsertData(
                table: "Tunings",
                columns: new[] { "Id", "Asset", "IsDeleted", "Name", "Notes" },
                values: new object[,]
                {
                    { 1, true, false, "Standard E", "E A D G B E" },
                    { 2, true, false, "Drop D", "D A D G B E" },
                    { 3, true, false, "C Standard", "C F A# D# G C" }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Asset", "BPM", "Difficulty", "IsDeleted", "Name", "TabNotation", "TuningId" },
                values: new object[,]
                {
                    { 1, true, 140, "Medium", false, "Alternate Picking en 3 cuerdas", "E|----------------|\r\nB|----------------|\r\nG|--0-2-4-2-0-----|\r\nD|--0-2-4-2-0-----|\r\nA|----------------|\r\nE|----------------|", 1 },
                    { 2, true, 120, "Easy", false, "Legato básico en escala menor", "E|----------------|\r\nB|----------------|\r\nG|--5h7p5---------|\r\nD|-------7h9p7----|\r\nA|----------------|\r\nE|----------------|", 1 },
                    { 3, true, 160, "Hard", false, "Sweep en arpegio mayor", "E|------12p8------|\r\nB|----10-----10----|\r\nG|--9---------9----|\r\nD|-----------------|\r\nA|-----------------|\r\nE|-----------------|", 2 },
                    { 4, true, 180, "Expert", false, "Alternate Picking avanzado", "E|--0-1-2-3-4--0-1-2-3-4--|\r\nB|------------------------|\r\nG|------------------------|\r\nD|------------------------|\r\nA|------------------------|\r\nE|------------------------|", 1 },
                    { 5, true, 150, "Hard", false, "Legato rápido con saltos", "E|--7h9p7--9h11p9--|\r\nB|------------------|\r\nG|--6h8p6-----------|\r\nD|------------------|\r\nA|------------------|\r\nE|------------------|", 1 }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name", "TechniqueId" },
                values: new object[,]
                {
                    { 1, true, "Serie de ejercicios para dominar el picking alternado a alta velocidad.", false, "Dominando Alternate Picking", 1 },
                    { 2, true, "Ejercicios de legato centrados en patrones menores y modales.", false, "Legato en escalas menores", 2 },
                    { 3, true, "Práctica de arpegios mayores con técnica de sweep.", false, "Sweep Arpegios Mayores", 3 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Asset", "Email", "GuitaristId", "IsDeleted", "Password", "Username" },
                values: new object[,]
                {
                    { 1, true, "anibalalvaradoandrade@gmail.com", 1, false, "$2a$12$tWJFKE5AxcI22akEUprqMuJoQ0QXLUbAg4FRklLYTQQc1dGCv1AlW", "anibal25" },
                    { 2, true, "carva@gmail.com", 2, false, "$2a$12$AyTpx8lEKlO9F9ltw5b8j.hGYJ44gK5pFL.ATJNYshNP6PswzOpPa", "carvaInsane" },
                    { 3, true, "viego@gmail.com", 3, false, "$2a$12$qq6J.RdSq1OTx8hLUGI5huNP9eL.zwD/Ev0QA2ZJ6W3z1YBW0Yo4W", "viego" }
                });

            migrationBuilder.InsertData(
                table: "GuitaristLessons",
                columns: new[] { "Id", "Asset", "GuitaristId", "IsDeleted", "LessonId", "ProgressPercent", "Status" },
                values: new object[,]
                {
                    { 1, true, 1, false, 1, 100.0, "Completed" },
                    { 2, true, 1, false, 2, 60.0, "InProgress" },
                    { 3, true, 2, false, 1, 40.0, "InProgress" },
                    { 4, true, 2, false, 3, 0.0, "NotStarted" },
                    { 5, true, 3, false, 2, 0.0, "NotStarted" }
                });

            migrationBuilder.InsertData(
                table: "LessonExercises",
                columns: new[] { "Id", "Asset", "ExerciseId", "IsDeleted", "LessonId" },
                values: new object[,]
                {
                    { 1, true, 1, false, 1 },
                    { 2, true, 4, false, 1 },
                    { 3, true, 2, false, 2 },
                    { 4, true, 5, false, 2 },
                    { 5, true, 3, false, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TuningId",
                table: "Exercises",
                column: "TuningId");

            migrationBuilder.CreateIndex(
                name: "IX_GuitaristLessons_GuitaristId",
                table: "GuitaristLessons",
                column: "GuitaristId");

            migrationBuilder.CreateIndex(
                name: "IX_GuitaristLessons_LessonId",
                table: "GuitaristLessons",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonExercises_ExerciseId",
                table: "LessonExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonExercises_LessonId",
                table: "LessonExercises",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TechniqueId",
                table: "Lessons",
                column: "TechniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GuitaristId",
                table: "Users",
                column: "GuitaristId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuitaristLessons");

            migrationBuilder.DropTable(
                name: "LessonExercises");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Guitarists");

            migrationBuilder.DropTable(
                name: "Tunings");

            migrationBuilder.DropTable(
                name: "Techniques");
        }
    }
}
