using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace MVCApplication.Data
{
    public class Seeding
    {
        private readonly string _conn;
        public Seeding(IConfiguration cfg) => _conn = cfg.GetConnectionString("Default") ?? "Data Source=app.db";
        public async Task SeedAdminUser()
        {
            using var s = new SqliteConnection(_conn);
            var exist = await s.ExecuteScalarAsync<int>("select count(1) from users where role = 'admin'");
            if (exist > 0) return;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
            var id = await s.ExecuteScalarAsync<int>(@"insert into users(Email, PasswordHash, Username, FullName, Role) values(@email, @hash, @username, @fullname, 'admin')  returning id",  new { email = "admin@example.com", hash = hashedPassword, username = "admin",fullname = "Admin User" });
        }
    }
}
