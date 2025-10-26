using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiStart.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
    {
            // Password 컬럼이 없으면 추가
   migrationBuilder.Sql(@"
          PRAGMA table_info(Users);
      ");
            
    // 컬럼 추가 (이미 있으면 무시)
    migrationBuilder.Sql(@"
   ALTER TABLE Users ADD COLUMN Password TEXT NULL;
    ", suppressTransaction: true);
      }

        /// <inheritdoc />
   protected override void Down(MigrationBuilder migrationBuilder)
        {
       migrationBuilder.Sql(@"
                ALTER TABLE Users DROP COLUMN Password;
      ", suppressTransaction: true);
        }
    }
}
