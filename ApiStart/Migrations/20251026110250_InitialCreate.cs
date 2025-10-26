using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ConsoleApp1.Migrations
{
 [DbContext(typeof(ApiStart.AppDbContext))]
 [Migration("20251026110250_InitialCreate")]
 public partial class InitialCreate : Migration
 {
 protected override void Up(MigrationBuilder migrationBuilder)
 {
 

 migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS Users (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Name TEXT NULL,
 Email TEXT NULL
);");
 }

 protected override void Down(MigrationBuilder migrationBuilder)
 {
 migrationBuilder.Sql("DROP TABLE IF EXISTS Todos;");
 migrationBuilder.Sql("DROP TABLE IF EXISTS Users;");
 }
 }
}
