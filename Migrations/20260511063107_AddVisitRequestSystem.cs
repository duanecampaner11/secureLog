using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureLog.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitRequestSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "VisitRequests",
                newName: "ReviewedByUserId");

            migrationBuilder.RenameColumn(
                name: "QueueNumber",
                table: "VisitRequests",
                newName: "ReturnReason");

            migrationBuilder.RenameColumn(
                name: "PersonToVisit",
                table: "VisitRequests",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserId",
                table: "VisitRequests",
                newName: "ConfirmationId");

            migrationBuilder.RenameColumn(
                name: "ApprovedAt",
                table: "VisitRequests",
                newName: "ReviewedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Company",
                table: "VisitRequests",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonToMeet",
                table: "VisitRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitTime",
                table: "VisitRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_ConfirmationId",
                table: "VisitRequests",
                column: "ConfirmationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_ConfirmationId",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "PersonToMeet",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "VisitTime",
                table: "VisitRequests");

            migrationBuilder.RenameColumn(
                name: "ReviewedByUserId",
                table: "VisitRequests",
                newName: "RejectionReason");

            migrationBuilder.RenameColumn(
                name: "ReviewedAt",
                table: "VisitRequests",
                newName: "ApprovedAt");

            migrationBuilder.RenameColumn(
                name: "ReturnReason",
                table: "VisitRequests",
                newName: "QueueNumber");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "VisitRequests",
                newName: "PersonToVisit");

            migrationBuilder.RenameColumn(
                name: "ConfirmationId",
                table: "VisitRequests",
                newName: "ApprovedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Company",
                table: "VisitRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
