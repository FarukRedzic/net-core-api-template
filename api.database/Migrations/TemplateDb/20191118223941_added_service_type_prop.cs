using Microsoft.EntityFrameworkCore.Migrations;

namespace api.database.Migrations.TemplateDb
{
    public partial class added_service_type_prop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "TemplateTable",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "TemplateTable");
        }
    }
}
