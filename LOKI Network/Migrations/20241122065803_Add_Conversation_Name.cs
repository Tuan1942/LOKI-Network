using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOKI_Network.Migrations
{
    /// <inheritdoc />
    public partial class Add_Conversation_Name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConversationName",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationName",
                table: "Conversations");
        }
    }
}
