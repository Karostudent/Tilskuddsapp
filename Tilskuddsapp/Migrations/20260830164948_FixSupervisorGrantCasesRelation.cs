using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tilskuddsapp.Migrations
{
    /// <inheritdoc />
    public partial class FixSupervisorGrantCasesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrantCases_Supervisors_SupervisorId1",
                table: "GrantCases");

            migrationBuilder.DropIndex(
                name: "IX_GrantCases_SupervisorId1",
                table: "GrantCases");

            migrationBuilder.DropColumn(
                name: "SupervisorId1",
                table: "GrantCases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisorId1",
                table: "GrantCases",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrantCases_SupervisorId1",
                table: "GrantCases",
                column: "SupervisorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GrantCases_Supervisors_SupervisorId1",
                table: "GrantCases",
                column: "SupervisorId1",
                principalTable: "Supervisors",
                principalColumn: "SupervisorId");
        }
    }
}
