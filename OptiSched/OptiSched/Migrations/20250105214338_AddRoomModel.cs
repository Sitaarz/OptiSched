using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OptiSched.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomNumber",
                table: "Schedules",
                newName: "RoomId");

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_RoomId",
                table: "Schedules",
                column: "RoomId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Room_RoomId",
                table: "Schedules",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Room_RoomId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_RoomId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Schedules",
                newName: "RoomNumber");
        }
    }
}
