using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class InitMainDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Firstname = table.Column<string>(type: "text", nullable: false),
                    Lastname = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormModule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormModule_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormModule_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolFormPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolFormPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolFormPermission_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolFormPermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolFormPermission_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolUser_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "Id", "Asset", "Description", "Name" },
                values: new object[] { 1, false, "Formulario principal", "Principal" });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Asset", "Description", "Name" },
                values: new object[] { 1, false, "Módulo de gestión", "Gestión" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Asset", "Description", "Name" },
                values: new object[,]
                {
                    { 1, false, "Permiso para ver", "Ver" },
                    { 2, false, "Permiso para editar", "Editar" },
                    { 3, false, "Permiso para eliminar", "Eliminar" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Asset", "Firstname", "Lastname", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, false, "Admin", "Principal", "111111111" },
                    { 2, false, "Usuario", "Demo", "222222222" }
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "Id", "Asset", "Description", "Name" },
                values: new object[,]
                {
                    { 1, false, "Rol de administrador", "Administrador" },
                    { 2, false, "Rol de usuario estándar", "Usuario" }
                });

            migrationBuilder.InsertData(
                table: "FormModule",
                columns: new[] { "Id", "Asset", "FormId", "ModuleId" },
                values: new object[] { 1, false, 1, 1 });

            migrationBuilder.InsertData(
                table: "RolFormPermission",
                columns: new[] { "Id", "Asset", "FormId", "PermissionId", "RolId" },
                values: new object[,]
                {
                    { 1, false, 1, 1, 1 },
                    { 2, false, 1, 2, 1 },
                    { 3, false, 1, 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Asset", "Email", "Password", "PersonId", "Username" },
                values: new object[,]
                {
                    { 1, false, "anibalalvaradoandrade@gmail.com", "123", 1, "admin" },
                    { 2, false, "usuario@demo.com", "usuario123", 2, "usuario" }
                });

            migrationBuilder.InsertData(
                table: "RolUser",
                columns: new[] { "Id", "Asset", "RolId", "UserId" },
                values: new object[,]
                {
                    { 1, false, 1, 1 },
                    { 2, false, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormModule_FormId",
                table: "FormModule",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormModule_ModuleId",
                table: "FormModule",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermission_FormId",
                table: "RolFormPermission",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermission_PermissionId",
                table: "RolFormPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermission_RolId",
                table: "RolFormPermission",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolUser_RolId",
                table: "RolUser",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolUser_UserId",
                table: "RolUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormModule");

            migrationBuilder.DropTable(
                name: "RolFormPermission");

            migrationBuilder.DropTable(
                name: "RolUser");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
