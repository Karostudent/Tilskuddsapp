using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tilskuddsapp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DoctorType = table.Column<int>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    SupervisorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SupervisorType = table.Column<int>(type: "INTEGER", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.SupervisorId);
                });

            migrationBuilder.CreateTable(
                name: "GrantCases",
                columns: table => new
                {
                    GrantCaseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DoctorId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorName = table.Column<string>(type: "TEXT", nullable: false),
                    SupervisorId = table.Column<int>(type: "INTEGER", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedGrant = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MaximumGrant = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    SupervisionHours = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    GrantCaseStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    SupervisorId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantCases", x => x.GrantCaseId);
                    table.ForeignKey(
                        name: "FK_GrantCases_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrantCases_Supervisors_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisors",
                        principalColumn: "SupervisorId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GrantCases_Supervisors_SupervisorId1",
                        column: x => x.SupervisorId1,
                        principalTable: "Supervisors",
                        principalColumn: "SupervisorId");
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrantCaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpenseType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_GrantCases_GrantCaseId",
                        column: x => x.GrantCaseId,
                        principalTable: "GrantCases",
                        principalColumn: "GrantCaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_GrantCaseId",
                table: "Expenses",
                column: "GrantCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantCases_DoctorId",
                table: "GrantCases",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantCases_SupervisorId",
                table: "GrantCases",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantCases_SupervisorId1",
                table: "GrantCases",
                column: "SupervisorId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "GrantCases");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Supervisors");
        }
    }
}
