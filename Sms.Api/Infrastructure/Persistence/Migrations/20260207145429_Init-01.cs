using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsTraffic.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientMessageHeader",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateCreation = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RemoteIp = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    DateSentToProvider = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Result = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Code = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    Response = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMessageHeader", x => x.Id);
                },
                comment: "Сообщения клиентов главная часть");

            migrationBuilder.CreateTable(
                name: "ClientMessageBody",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClientMessageHeaderId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    SmsId = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMessageBody", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMessageBody_ClientMessageHeader_ClientMessageHeaderId",
                        column: x => x.ClientMessageHeaderId,
                        principalTable: "ClientMessageHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Сообщения клиентов дочерняя часть");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageBody_ClientMessageHeaderId",
                table: "ClientMessageBody",
                column: "ClientMessageHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageBody_Phone",
                table: "ClientMessageBody",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageBody_SmsId",
                table: "ClientMessageBody",
                column: "SmsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageHeader_DateCreation",
                table: "ClientMessageHeader",
                column: "DateCreation");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessageHeader_DateSentToProvider",
                table: "ClientMessageHeader",
                column: "DateSentToProvider");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMessageBody");

            migrationBuilder.DropTable(
                name: "ClientMessageHeader");
        }
    }
}
