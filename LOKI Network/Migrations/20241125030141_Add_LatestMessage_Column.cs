using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOKI_Network.Migrations
{
    /// <inheritdoc />
    public partial class Add_LatestMessage_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LatestMessageDate",
                table: "Conversations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestMessageDate",
                table: "Conversations");
        }
    }
}
