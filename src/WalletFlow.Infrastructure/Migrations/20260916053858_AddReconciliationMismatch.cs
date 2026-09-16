using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReconciliationMismatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReconciliationMismatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActualBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpectedBalanceFromLedger = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Difference = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DetectedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationMismatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationMismatches_WalletId",
                table: "ReconciliationMismatches",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReconciliationMismatches");
        }
    }
}
