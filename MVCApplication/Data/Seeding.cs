using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using MVCApplication.Models.Seeding;

namespace MVCApplication.Data
{
    public class Seeding
    {
        private readonly AdminSettings _adminSettings;
        private readonly string _conn;
        public Seeding(IConfiguration cfg, IOptions<AdminSettings> adminSettings) => (_conn, _adminSettings) = (cfg.GetConnectionString("Default") ?? "Data Source=app.db", adminSettings.Value);  
        public async Task SeedAdminUser()
        {
            using var s = new SqliteConnection(_conn);
            var exist = await s.ExecuteScalarAsync<int>("SELECT count(1) FROM users WHERE role = 'admin'");
            if (exist > 0) return;

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(_adminSettings.SeedPassword);
            var email = _adminSettings.SeedEmail;
            var id = await s.ExecuteScalarAsync<int>(@"INSERT INTO users(Email, PasswordHash, Username, FullName, Role) VALUES (@Email, @Hash, @Username, @FullName, 'admin')  RETURNING id",  new { Email = email, Hash = hashedPassword, Username = "admin", FullName = "Admin User" });
        }
    }
}
