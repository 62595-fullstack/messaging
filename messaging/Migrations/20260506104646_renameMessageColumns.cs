using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace messaging.Migrations
{
    /// <inheritdoc />
    public partial class renameMessageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Message",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "Message",
                newName: "userSenderId");

            migrationBuilder.RenameColumn(
                name: "ReceiverUserId",
                table: "Message",
                newName: "userReviverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content",
                table: "Message",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "userSenderId",
                table: "Message",
                newName: "SenderUserId");

            migrationBuilder.RenameColumn(
                name: "userReviverId",
                table: "Message",
                newName: "ReceiverUserId");
        }
    }
}
