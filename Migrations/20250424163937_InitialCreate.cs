using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op: we're syncing to existing DB
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op
        }
    }
}