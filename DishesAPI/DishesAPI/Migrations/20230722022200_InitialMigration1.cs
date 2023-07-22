using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DishesAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DishIngredient",
                keyColumns: new[] { "DishesId", "IngredientsId" },
                keyValues: new object[] { new Guid("b512d7cf-b331-4b54-8dae-d1228d128e8d"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35") });

            migrationBuilder.DeleteData(
                table: "DishIngredient",
                keyColumns: new[] { "DishesId", "IngredientsId" },
                keyValues: new object[] { new Guid("fd630a57-2352-4731-b25c-db9cc7601b16"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DishIngredient",
                columns: new[] { "DishesId", "IngredientsId" },
                values: new object[,]
                {
                    { new Guid("b512d7cf-b331-4b54-8dae-d1228d128e8d"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35") },
                    { new Guid("fd630a57-2352-4731-b25c-db9cc7601b16"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35") }
                });
        }
    }
}
