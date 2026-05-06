using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace messaging.Migrations
{
    /// <inheritdoc />
    public partial class cleanmessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_userReviverEmail",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_userReviverIdEmail",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_userSenderEmail",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_userSenderIdEmail",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_userReviverEmail",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_userReviverIdEmail",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_userSenderEmail",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_userSenderIdEmail",
                table: "Message");

            migrationBuilder.DropColumn(name: "userReviverEmail", table: "Message");
            migrationBuilder.DropColumn(name: "userReviverIdEmail", table: "Message");
            migrationBuilder.DropColumn(name: "userSenderEmail", table: "Message");
            migrationBuilder.DropColumn(name: "userSenderIdEmail", table: "Message");
            migrationBuilder.DropColumn(name: "content", table: "Message");

            migrationBuilder.AddColumn<string>(
                name: "SenderUserId",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserId",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "SenderUserId", table: "Message");
            migrationBuilder.DropColumn(name: "ReceiverUserId", table: "Message");
            migrationBuilder.DropColumn(name: "Content", table: "Message");

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userReviverEmail",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userReviverIdEmail",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userSenderEmail",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userSenderIdEmail",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Message_userReviverEmail",
                table: "Message",
                column: "userReviverEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Message_userReviverIdEmail",
                table: "Message",
                column: "userReviverIdEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Message_userSenderEmail",
                table: "Message",
                column: "userSenderEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Message_userSenderIdEmail",
                table: "Message",
                column: "userSenderIdEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_userReviverEmail",
                table: "Message",
                column: "userReviverEmail",
                principalTable: "User",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_userReviverIdEmail",
                table: "Message",
                column: "userReviverIdEmail",
                principalTable: "User",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_userSenderEmail",
                table: "Message",
                column: "userSenderEmail",
                principalTable: "User",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_userSenderIdEmail",
                table: "Message",
                column: "userSenderIdEmail",
                principalTable: "User",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
