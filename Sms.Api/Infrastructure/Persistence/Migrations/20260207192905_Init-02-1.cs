using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsTraffic.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init021 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientMessageBody_ClientMessageHeader_ClientMessageHeaderId",
                table: "ClientMessageBody");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "ClientMessageHeader",
                newName: "RESPONSE");

            migrationBuilder.RenameColumn(
                name: "RemoteIp",
                table: "ClientMessageHeader",
                newName: "REMOTE_IP");

            migrationBuilder.RenameColumn(
                name: "DateSentToProvider",
                table: "ClientMessageHeader",
                newName: "DATE_SENT_TO_PROVIDER");

            migrationBuilder.RenameColumn(
                name: "DateCreation",
                table: "ClientMessageHeader",
                newName: "DATE_CREATION");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ClientMessageHeader",
                newName: "ID_MESSAGE_HEADER");

            migrationBuilder.RenameColumn(
                name: "Result",
                table: "ClientMessageHeader",
                newName: "RESPONSE_RESULT");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ClientMessageHeader",
                newName: "RESPONSE_DESCRIPTION");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ClientMessageHeader",
                newName: "RESPONSE_CODE");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageHeader_DateSentToProvider",
                table: "ClientMessageHeader",
                newName: "IX_ClientMessageHeader_DATE_SENT_TO_PROVIDER");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageHeader_DateCreation",
                table: "ClientMessageHeader",
                newName: "IX_ClientMessageHeader_DATE_CREATION");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "ClientMessageBody",
                newName: "PHONE");

            migrationBuilder.RenameColumn(
                name: "SmsId",
                table: "ClientMessageBody",
                newName: "SMS_ID");

            migrationBuilder.RenameColumn(
                name: "ClientMessageHeaderId",
                table: "ClientMessageBody",
                newName: "ID_MESSAGE_HEADER");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ClientMessageBody",
                newName: "ID_MESSAGE_BODY");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageBody_Phone",
                table: "ClientMessageBody",
                newName: "IX_ClientMessageBody_PHONE");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageBody_SmsId",
                table: "ClientMessageBody",
                newName: "IX_ClientMessageBody_SMS_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageBody_ClientMessageHeaderId",
                table: "ClientMessageBody",
                newName: "IX_ClientMessageBody_ID_MESSAGE_HEADER");

            migrationBuilder.AddColumn<string>(
                name: "MESSAGE",
                table: "ClientMessageHeader",
                type: "NVARCHAR2(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "STATUS",
                table: "ClientMessageHeader",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageHeader_REMOTE_IP",
                table: "ClientMessageHeader",
                column: "REMOTE_IP");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMessageBody_ClientMessageHeader_ID_MESSAGE_HEADER",
                table: "ClientMessageBody",
                column: "ID_MESSAGE_HEADER",
                principalTable: "ClientMessageHeader",
                principalColumn: "ID_MESSAGE_HEADER",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientMessageBody_ClientMessageHeader_ID_MESSAGE_HEADER",
                table: "ClientMessageBody");

            migrationBuilder.DropIndex(
                name: "IX_ClientMessageHeader_REMOTE_IP",
                table: "ClientMessageHeader");

            migrationBuilder.DropColumn(
                name: "MESSAGE",
                table: "ClientMessageHeader");

            migrationBuilder.DropColumn(
                name: "STATUS",
                table: "ClientMessageHeader");

            migrationBuilder.RenameColumn(
                name: "RESPONSE",
                table: "ClientMessageHeader",
                newName: "Response");

            migrationBuilder.RenameColumn(
                name: "REMOTE_IP",
                table: "ClientMessageHeader",
                newName: "RemoteIp");

            migrationBuilder.RenameColumn(
                name: "DATE_SENT_TO_PROVIDER",
                table: "ClientMessageHeader",
                newName: "DateSentToProvider");

            migrationBuilder.RenameColumn(
                name: "DATE_CREATION",
                table: "ClientMessageHeader",
                newName: "DateCreation");

            migrationBuilder.RenameColumn(
                name: "ID_MESSAGE_HEADER",
                table: "ClientMessageHeader",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RESPONSE_RESULT",
                table: "ClientMessageHeader",
                newName: "Result");

            migrationBuilder.RenameColumn(
                name: "RESPONSE_DESCRIPTION",
                table: "ClientMessageHeader",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "RESPONSE_CODE",
                table: "ClientMessageHeader",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageHeader_DATE_SENT_TO_PROVIDER",
                table: "ClientMessageHeader",
                newName: "IX_ClientMessageHeader_DateSentToProvider");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageHeader_DATE_CREATION",
                table: "ClientMessageHeader",
                newName: "IX_ClientMessageHeader_DateCreation");

            migrationBuilder.RenameColumn(
                name: "PHONE",
                table: "ClientMessageBody",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "SMS_ID",
                table: "ClientMessageBody",
                newName: "SmsId");

            migrationBuilder.RenameColumn(
                name: "ID_MESSAGE_HEADER",
                table: "ClientMessageBody",
                newName: "ClientMessageHeaderId");

            migrationBuilder.RenameColumn(
                name: "ID_MESSAGE_BODY",
                table: "ClientMessageBody",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageBody_PHONE",
                table: "ClientMessageBody",
                newName: "IX_ClientMessageBody_Phone");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageBody_SMS_ID",
                table: "ClientMessageBody",
                newName: "IX_ClientMessageBody_SmsId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMessageBody_ID_MESSAGE_HEADER",
                table: "ClientMessageBody",
                newName: "IX_ClientMessageBody_ClientMessageHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMessageBody_ClientMessageHeader_ClientMessageHeaderId",
                table: "ClientMessageBody",
                column: "ClientMessageHeaderId",
                principalTable: "ClientMessageHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
