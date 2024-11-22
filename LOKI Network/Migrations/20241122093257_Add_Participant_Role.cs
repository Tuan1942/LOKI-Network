using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOKI_Network.Migrations
{
    /// <inheritdoc />
    public partial class Add_Participant_Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "ConversationParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ConversationParticipants");
        }
    }
}
