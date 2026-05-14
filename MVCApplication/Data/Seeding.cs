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
            var id = await s.ExecuteScalarAsync<int>(@"INSERT INTO users(Email, PasswordHash, Username, FullName, Role) VALUES (@Email, @Hash, @Username, @FullName, 'admin')  RETURNING id", new { Email = email, Hash = hashedPassword, Username = "admin", FullName = "Admin User" });
        }

        public async Task SeedUser()
        {
            using var conn = new SqliteConnection(_conn);
            var exist = await conn.ExecuteScalarAsync<int>("SELECT count(1) FROM users WHERE role = 'user'");
            if (exist > 0) return;

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("user123");
            string email = "user@example.com";
            int id = await conn.ExecuteScalarAsync<int>(
                @"INSERT INTO users(Email, PasswordHash, Username, FullName, Role)
                VALUES (@Email, @Hash, @Username, @FullName, 'user')  
                RETURNING id", 
                new 
                { 
                    Email = email, 
                    Hash = hashedPassword, 
                    Username = "TestUser", 
                    FullName = "Test User"
                });
        }


        public async Task SeedFeedback()
        {
            using var conn = new SqliteConnection(_conn);
            var exist = await conn.ExecuteScalarAsync<int>("SELECT count(1) FROM feedback");
            if (exist > 0) return;

            int id = await conn.ExecuteScalarAsync<int>(
                @"INSERT INTO feedback(FullName, Email, Type, Heading, Message, WantsContact) 
                VALUES (@FullName, @Email, @Type, @Heading, @Message, @WantsContact)
                RETURNING id", 
                new
                { 
                    FullName = "Test User", 
                    Email = "user@example.com", 
                    Type = "Comment", 
                    Heading = "Great service!",
                    Message = "Great service!", 
                    WantsContact = 0
                });
        }

        public async Task SeedEvent()
        {
            using var conn = new SqliteConnection(_conn);
            var exist = await conn.ExecuteScalarAsync<int>("SELECT count(1) FROM events");
            if (exist > 0) return;


            int id = await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO events (Title, Description, Location, EventDate) 
            VALUES (@Title, @Description, @Location, @EventDate) 
            RETURNING Id",
            new
            {
                Title = "Community Meetup",
                Description = "A casual meetup for local community members.",
                Location = "Town Hall, Perth",
                EventDate = DateTime.UtcNow.AddDays(14)
            });
        }

        public async Task SeedBooking()
        {
            using var conn = new SqliteConnection(_conn);
            var exist = await conn.ExecuteScalarAsync<int>("SELECT count(1) FROM bookings");
            if (exist > 0) return;


            int id = await conn.ExecuteScalarAsync<int>(
                @"INSERT INTO bookings(FullName, Email, BookingDate) 
                VALUES (@FullName, @Email, @BookingDate)
                RETURNING id",
                new
                {
                    FullName = "Test User",
                    Email = "user@example.com",
                    BookingDate = DateTime.UtcNow
                });
        }

        public async Task SeedAnnouncements()
        {
            //TODO: Implement seeding for announcements
            await Task.CompletedTask;
        }
    }
}
