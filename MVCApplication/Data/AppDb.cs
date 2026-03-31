using Microsoft.Data.Sqlite;
using BCrypt.Net;
using Dapper;
using MVCApplication.Models;

namespace MVCApplication.Data
{
    public class AppDb
    {
        private readonly string _conn;

        public AppDb(IConfiguration cfg)
        {
            _conn = cfg.GetConnectionString("Default") ?? "Data Source=app.db";
        }

        public async Task<User?> Login(string password, string email)
        {
            using var c = new SqliteConnection(_conn);

            var user = await c.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE email = @email",
                new { email });

            if (user == null)
                return null;

            return BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash)
                ? user
                : null;
        }

        public async Task<User?> Register(string password, string email, string username, string fullname, string role)
        {
            using var c = new SqliteConnection(_conn);

            var UserExist = await c.ExecuteScalarAsync<int>(
                "SELECT count(1) FROM users WHERE email = @email",
                new { email }
                );
            if (UserExist > 0 ) return null;

            var id = await c.ExecuteScalarAsync<int>(@"
                INSERT INTO users (email, password_hash, username, fullname, role) VALUES (@email, @hash, @username, @fullname, @role) RETURNING id", 
                new { email, hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password), username, fullname, role });

            return await c.QueryFirstOrDefaultAsync(
                "SELECT * FROM users WHERE id = @id", new {id});
        }
        public async Task EnsureCreated()
        {
            using var c = new SqliteConnection(_conn);
            
            await c.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS users (
                id            integer primary key autoincrement,
                email         text unique not null,
                password_hash text not null,
                username      text not null,
                fullname      text not null,
                role          text not null default 'user',
                created_at    datetime default current_timestamp
            );
            CREATE TABLE IF NOT EXISTS events (
                id              integer primary key autoincrement,
                title           text not null,
                description     text not null,
                location        text not null,
                date            datetime default current_timestamp
            );
            CREATE TABLE IF NOT EXISTS feedback (
                id             integer  primary key autoincrement,
                fullname       text     not null,
                email          text     not null,
                type           text     not null,
                heading        text     not null,
                message        text     not null,
                wants_contact  integer  not null default 0,
                submitted_date datetime default current_timestamp
            );
            CREATE TABLE IF NOT EXISTS bookings (
                id              integer primary key autoincrement,
                fullname        text not null,
                email           text not null,
                date            datetime
            );
        ");
        }
        public async Task<(List<User>?, User?)> GetUser(int? id)
        {
            using var c = new SqliteConnection(_conn);
            return id is not null ? 
                (new List<User>(), (await c.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id", new { id }))) : ((await c.QueryAsync<User>("SELECT * FROM users ORDER BY created_at ASC")).ToList(), null);
        }
        public async Task<(List<Event>?, Event?)> GetEvent(int? id)
        {
            using var c = new SqliteConnection(_conn);
            return id is not null ? 
                (new List<Event>(), (await c.QueryFirstOrDefaultAsync<Event>("SELECT * FROM events WHERE id = @id", new { id }))) : ((await c.QueryAsync<Event>("SELECT * FROM events ORDER BY date ASC")).ToList(), null);
        }
        public async Task<(List<Booking>?, Booking?)> GetBooking(int? id)
        {
            using var c = new SqliteConnection(_conn);
            return id is not null ? 
                (new List<Booking>(), (await c.QueryFirstOrDefaultAsync<Booking>("SELECT * FROM bookings WHERE id = @id", new { id }))) : ((await c.QueryAsync<Booking>("SELECT * FROM bookings ORDER BY date ASC")).ToList(), null);
        }
        public async Task<(List<Feedback>?, Feedback?)> GetFeedback(int? id)
        {
            using var c = new SqliteConnection(_conn);
            return id is not null ? 
                (new List<Feedback>(), (await c.QueryFirstOrDefaultAsync<Feedback>("SELECT * FROM feedback WHERE id = @id", new { id }))) : ((await c.QueryAsync<Feedback>("SELECT * FROM feedback ORDER BY submitted_date ASC")).ToList(), null);
        }
        public async Task<bool> SaveUser(User user)
        {
            using var c = new SqliteConnection(_conn);
            return await c.ExecuteAsync(@"INSERT INTO users(email , password_hash, username, fullname, role) VALUES (@email, @password_hash, @username, @fullname, @role)", user) > 0;
        }
        public async Task<bool> SaveFeedback(Feedback feedback)
        {
            using var c = new SqliteConnection(_conn);
            return await c.ExecuteAsync(@"INSERT INTO feedback(fullname, email, type, heading, message, wants_contact, submitted_date) VALUES (@FullName, @Email, @Type, @Heading, @Message, @WantsContact, @SubmittedDate)", feedback) > 0;
        }
        public async Task<bool> SaveEvent(Event _event)
        {
            using var c = new SqliteConnection(_conn);
            return await c.ExecuteAsync(@"INSERT INTO events(title , description, location, date) VALUES (@title, @description, @location, @date)", _event) > 0;
        }
        public async Task<bool> SaveBooking(Booking booking)
        {
            using var c = new SqliteConnection(_conn);
            return await c.ExecuteAsync(@"INSERT INTO bookings(fullname , email, date) VALUES (@FullName, @Email, @Date)", booking) > 0;
        }
        public async Task<int> UpdateUser(List<User> users)
        {
            if (users == null || users.Count == 0) return 0;
            using var c = new SqliteConnection(_conn);            
            int affected = 0;
            foreach (var user in users)
            {
                var sets = new List<string>();
                if(user.FullName is not null)sets.Add("fullname = @FullName");
                if (user.Email is not null) sets.Add("email = @Email");
                if (user.Role is not null) sets.Add("role = @Role");
                if (user.Username is not null) sets.Add("username = @Username");

                if (sets.Count == 0) continue; // No fields to update, skip this user

                affected += await c.ExecuteAsync($"UPDATE users SET {string.Join(",", sets)} WHERE id = @Id", user);
            }
            return affected;
        }
        public async Task<int> UpdateFeedback(List<Feedback> feedbacks)
        {
            if (feedbacks == null || feedbacks.Count == 0) return 0;
            using var c = new SqliteConnection(_conn);
            int affected = 0;
            foreach (var feed in feedbacks)
            {
                var sets = new List<string>();
                if (feed.Heading is not null) sets.Add("heading = @Heading");
                if (feed.FullName is not null) sets.Add("fullname = @Fullname");
                if (feed.Type is not null) sets.Add("type = @Type");
                if (feed.Email is not null) sets.Add("email = @Email");
                if (feed.Message is not null) sets.Add("message = @Message");

                if (sets.Count == 0) continue; // No fields to update, skip this feedback

                affected += await c.ExecuteAsync($"UPDATE feedback SET {string.Join(",", sets)} WHERE id = @Id", feed);
            }
            return affected;
        }
        public async Task<int> UpdateEvent(List<Event> _event)
        {
            if (_event == null || _event.Count == 0) return 0;
            using var c = new SqliteConnection(_conn);
            int affected = 0;
            foreach (var eve in _event)
            {
                var sets = new List<string>();
                if (eve.Title is not null) sets.Add("title = @Title");
                if (eve.Description is not null) sets.Add("description = @Description");
                if (eve.Location is not null) sets.Add("location = @Location");
                if (eve.EventDate != DateTime.MinValue) sets.Add("date = @EventDate");

                if (sets.Count == 0) continue; // No fields to update, skip this event

                affected += await c.ExecuteAsync($"UPDATE events SET {string.Join(",", sets)} WHERE id = @Id", eve);
            }
            return affected;
        }
        public async Task<int> UpdateBooking(List<Booking> bookings)
        {
            if (bookings == null || bookings.Count == 0) return 0;
            using var c = new SqliteConnection(_conn);
            int affected = 0;
            foreach (var booking in bookings)
            {
                var sets = new List<string>();
                if (booking.FullName is not null) sets.Add("fullname = @FullName");
                if (booking.Email is not null) sets.Add("email = @Email");
                if (booking.BookingDate != DateTime.MinValue) sets.Add("date = @BookingDate");

                if (sets.Count == 0) continue; // No fields to update, skip this booking

                affected += await c.ExecuteAsync($"UPDATE bookings SET {string.Join(",", sets)} WHERE id = @Id", booking);
            }
            return affected;
        }
        public async Task<bool> DeleteUsers(List<int> id)
        {
            if (id == null || id.Count == 0) return false;
            using var c = new SqliteConnection(_conn);
            return (await c.ExecuteAsync("DELETE FROM users WHERE id IN @Ids", new { Ids = id }) > 0 );
        }
        public async Task<bool> DeleteFeedbacks(List<int> id)
        {
            if (id == null || id.Count == 0) return false;
            using var c = new SqliteConnection(_conn);
            return (await c.ExecuteAsync("DELETE FROM feedback WHERE id IN @Ids", new { Ids = id }) > 0);
        }
        public async Task<bool> DeleteEvents(List<int> id)
        {
            if (id == null || id.Count == 0) return false;
            using var c = new SqliteConnection(_conn);
            return (await c.ExecuteAsync("DELETE FROM events WHERE id IN @Ids", new { Ids = id }) > 0);
        }
        public async Task<bool> DeleteBookings(List<int> id)
        {
            if (id == null || id.Count == 0) return false;
            using var c = new SqliteConnection(_conn);
            return (await c.ExecuteAsync("DELETE FROM bookings WHERE id IN @Ids", new { Ids = id }) > 0);
        }
    }
}
