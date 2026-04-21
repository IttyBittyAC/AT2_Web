using Microsoft.Data.Sqlite;
using BCrypt.Net;
using Dapper;
using MVCApplication.Models;

namespace MVCApplication.Data
{
    public class AppDb
    {
        private readonly string _conn; //Connection string for SQLite database
        private readonly ILogger _logger; //Logger for error handling

        public AppDb(IConfiguration cfg, ILogger<AppDb> logger)
        {
            _conn = cfg.GetConnectionString("Default") ?? "Data Source=app.db";
            _logger = logger;
        }

        public async Task<User?> Login(string password, string email)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Fetch user by email
                User? user = await con.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE email = @email",
                    new { email });

                if(user == null)
                    return null;

                // Verify password using BCrypt
                return BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash) 
                    ? user 
                    : null;
            }
            catch (SqliteException ex) 
            {
                _logger.LogError(ex, "Database error during login for {Email}", email);
                return null;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unexpected error during login");
                throw;
            }
        }

        public async Task<User?> Register(string password, string email, string username, string fullname, string role)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Check if user with the same email already exists
                int UserExist = await con.ExecuteScalarAsync<int>(
                    "SELECT count(1) FROM users WHERE email = @email",
                    new { email });

                if (UserExist > 0) 
                    return null;

                //  Insert new user and return the created user
                int id = await con.ExecuteScalarAsync<int>(
                    @"INSERT INTO users (email, password_hash, username, fullname, role) VALUES (@email, @hash, @username, @fullname, @role) RETURNING id",
                    new { email, hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password), username, fullname, role });

                return await con.QueryFirstOrDefaultAsync(
                    "SELECT * FROM users WHERE id = @id", new { id });
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during registration for {Email}", email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for {Email}", email);
                throw;
            }
        }

        public async Task EnsureCreated()
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Create tables if they do not exist
                await con.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS users (
                        Id            integer primary key autoincrement,
                        Email         text unique not null,
                        PasswordHash  text not null,
                        Username      text not null,
                        FullName      text not null,
                        Role          text not null default 'user',
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
                    );"
                );
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during EnsureCreated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during EnsureCreated");
            }
        }

        public async Task<(List<User>?, User?)> GetUser(int? id)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // If id is provided, return the specific user, otherwise return all users
                return id is not null ?
                    (new List<User>(), (await con.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id", new { id }))) : ((await con.QueryAsync<User>("SELECT * FROM users ORDER BY created_at ASC")).ToList(), null);
            }
            catch (SqliteException ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Database error during GetUser with id {Id}", id);
                else
                    _logger.LogError(ex, "Database error during GetUser for all users");
                return (null, null);
            }
            catch (Exception ex) 
            {
                if (id != null)
                    _logger.LogError(ex, "Unexpected error during GetUser with id {Id}", id);
                else
                    _logger.LogError(ex, "Unexpected error during GetUser for all users");
                throw;
            }
        }

        public async Task<(List<User>?, User?)> GetUserByEmail(string? email)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return user by email
                return (new List<User>(), await con.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE email = @Email", new { Email = email }));
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during GetUserByEmail for {Email}", email);
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during GetUserByEmail for {Email}", email);
                throw;
            }
        }

        public async Task<(List<Event>?, Event?)> GetEvent(int? id)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // If id is provided, return the specific event, otherwise return all events
                return id is not null ?
                    (new List<Event>(), (await con.QueryFirstOrDefaultAsync<Event>("SELECT * FROM events WHERE id = @id", new { id }))) : ((await con.QueryAsync<Event>("SELECT * FROM events ORDER BY date ASC")).ToList(), null);
            }
            catch (SqliteException ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Database error during GetEvent with id {Id}", id);
                else
                    _logger.LogError(ex, "Database error during GetEvent for all events");
                return (null, null);
            }
            catch (Exception ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Unexpected error during GetEvent with id {Id}", id);
                else
                    _logger.LogError(ex, "Unexpected error during GetEvent for all events");
                throw;
            }   
        }

        public async Task<(List<Booking>?, Booking?)> GetBooking(int? id)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // If id is provided, return the specific booking, otherwise return all bookings
                return id is not null ?
                    (new List<Booking>(), (await con.QueryFirstOrDefaultAsync<Booking>("SELECT * FROM bookings WHERE id = @id", new { id }))) : ((await con.QueryAsync<Booking>("SELECT * FROM bookings ORDER BY date ASC")).ToList(), null);
            }
            catch (SqliteException ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Database error during GetBooking with id {Id}", id);
                else
                    _logger.LogError(ex, "Database error during GetBooking for all bookings");
                return (null, null);
            }
            catch (Exception ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Unexpected error during GetBooking with id {Id}", id);
                else
                    _logger.LogError(ex, "Unexpected error during GetBooking for all bookings");
                throw;
            }
        }

        public async Task<(List<Booking>?, Booking?)> GetBookingByEmail(string? email)
        {
            try
            {
                using var c = new SqliteConnection(_conn);

                // Return bookings by email
                return ((await c.QueryAsync<Booking>("SELECT * FROM bookings WHERE email = @Email ORDER BY date ASC", new { Email = email })).ToList(), null);
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during GetBookingByEmail for {Email}", email);
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during GetBookingByEmail for {Email}", email);
                throw;
            }
        }

        public async Task<(List<Feedback>?, Feedback?)> GetFeedback(int? id)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // If id is provided, return the specific feedback, otherwise return all feedback
                return id is not null ?
                    (new List<Feedback>(), (await con.QueryFirstOrDefaultAsync<Feedback>("SELECT * FROM feedback WHERE id = @id", new { id }))) : ((await con.QueryAsync<Feedback>("SELECT * FROM feedback ORDER BY submitted_date ASC")).ToList(), null);
            }
            catch (SqliteException ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Database error during GetFeedback with id {Id}", id);
                else
                    _logger.LogError(ex, "Database error during GetFeedback for all feedback");
                return (null, null);
            }
            catch (Exception ex)
            {
                if (id != null)
                    _logger.LogError(ex, "Unexpected error during GetFeedback with id {Id}", id);
                else
                    _logger.LogError(ex, "Unexpected error during GetFeedback for all feedback");
                throw;
            }

        }
        public async Task<bool> SaveUser(User user)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);
                
                // Return true if at least one row was affected (i.e., user was inserted)
                return await con.ExecuteAsync(@"
                    INSERT INTO users(Email , PasswordHash, Username, FullName, Role) 
                    VALUES (@Email, @PasswordHash, @Username, @FullName, @Role)", user) > 0; 
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during SaveUser for email {Email}", user.Email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during SaveUser for email {Email}", user.Email);
                throw;
            }
        }

        public async Task<bool> SaveFeedback(Feedback feedback)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., feedback was inserted)
                return await con.ExecuteAsync(@"INSERT INTO feedback(fullname, email, type, heading, message, wants_contact, submitted_date) VALUES (@FullName, @Email, @Type, @Heading, @Message, @WantsContact, @SubmittedDate)", feedback) > 0;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during SaveFeedback for email {Email}", feedback.Email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during SaveFeedback for email {Email}", feedback.Email);
                throw;
            }
        }

        public async Task<bool> SaveEvent(Event _event)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., event was inserted)
                return await con.ExecuteAsync(@"INSERT INTO events(title , description, location, date) VALUES (@title, @description, @location, @date)", _event) > 0;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during SaveEvent for title {Title}", _event.Title);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during SaveEvent for title {Title}", _event.Title);
                throw;
            }
        }

        public async Task<bool> SaveBooking(Booking booking)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., booking was inserted)
                return await con.ExecuteAsync(@"INSERT INTO bookings(fullname , email, date) VALUES (@FullName, @Email, @Date)", booking) > 0;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during SaveBooking for email {Email}", booking.Email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during SaveBooking for email {Email}", booking.Email);
                throw;
            }
        }

        public async Task<int> UpdateUser(List<User> users)
        {
            if (users == null || users.Count == 0) return 0;

            try
            {    
                using SqliteConnection con = new SqliteConnection(_conn);
                int affected = 0;

                // Loop through each user and build dynamic SQL for updating only the provided fields
                foreach (User user in users)
                {
                    List<string> sets = new List<string>();
                    if (user.FullName is not null) sets.Add("FullName = @FullName");
                    if (user.Email is not null) sets.Add("Email = @Email");
                    if (user.Role is not null) sets.Add("Role = @Role");
                    if (user.Username is not null) sets.Add("Username = @Username");

                    if (sets.Count == 0) continue; // No fields to update, skip this user

                    // Execute the update statement for the user
                    affected += await con.ExecuteAsync($"UPDATE users SET {string.Join(",", sets)} WHERE id = @Id", user);
                }
                return affected;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during UpdateUser");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during UpdateUser");
                throw;
            }
        }

        public async Task<int> UpdateFeedback(List<Feedback> feedbacks)
        {
            if (feedbacks == null || feedbacks.Count == 0) return 0;

            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);
                int affected = 0;

                // Loop through each feedback and build dynamic SQL for updating only the provided fields
                foreach (Feedback feed in feedbacks)
                {
                    List<string> sets = new List<string>();
                    if (feed.Heading is not null) sets.Add("heading = @Heading");
                    if (feed.FullName is not null) sets.Add("fullname = @Fullname");
                    if (feed.Type is not null) sets.Add("type = @Type");
                    if (feed.Email is not null) sets.Add("email = @Email");
                    if (feed.Message is not null) sets.Add("message = @Message");

                    if (sets.Count == 0) continue; // No fields to update, skip this feedback

                    //Execute the update statement for the feedback
                    affected += await con.ExecuteAsync($"UPDATE feedback SET {string.Join(",", sets)} WHERE id = @Id", feed);
                }
                return affected;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during UpdateFeedback");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during UpdateFeedback");
                throw;
            }
        }

        public async Task<int> UpdateEvent(List<Event> _event)
        {
            if (_event == null || _event.Count == 0) return 0;

            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);
                int affected = 0;

                // Loop through each event and build dynamic SQL for updating only the provided fields
                foreach (Event eve in _event)
                {
                    List<string> sets = new List<string>();
                    if (eve.Title is not null) sets.Add("title = @Title");
                    if (eve.Description is not null) sets.Add("description = @Description");
                    if (eve.Location is not null) sets.Add("location = @Location");
                    if (eve.EventDate != DateTime.MinValue) sets.Add("date = @EventDate");

                    if (sets.Count == 0) continue; // No fields to update, skip this event

                    //Execute the update statement for the event
                    affected += await con.ExecuteAsync($"UPDATE events SET {string.Join(",", sets)} WHERE id = @Id", eve);
                }
                return affected;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during UpdateEvent");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during UpdateEvent");
                throw;
            }
        }

        public async Task<int> UpdateBooking(List<Booking> bookings)
        { 
            if (bookings == null || bookings.Count == 0) return 0;

            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);
                int affected = 0;

                // Loop through each booking and build dynamic SQL for updating only the provided fields
                foreach (Booking booking in bookings)
                {
                    List<string> sets = new List<string>();
                    if (booking.FullName is not null) sets.Add("fullname = @FullName");
                    if (booking.Email is not null) sets.Add("email = @Email");
                    if (booking.BookingDate != DateTime.MinValue) sets.Add("date = @BookingDate");

                    if (sets.Count == 0) continue; // No fields to update, skip this booking

                    //Execute the update statement for the booking
                    affected += await con.ExecuteAsync($"UPDATE bookings SET {string.Join(",", sets)} WHERE id = @Id", booking);
                }
                return affected;
            }
            catch (SqliteException ex) 
            {
                _logger.LogError(ex, "Database error during UpdateBooking");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during UpdateBooking");
                throw;
            }
        }

        public async Task<bool> DeleteUsers(List<int> id)
        {
            if (id == null || id.Count == 0) return false;

            try
            {    
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., user(s) were deleted)
                return (await con.ExecuteAsync("DELETE FROM users WHERE id IN @Ids", new { Ids = id }) > 0);
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during DeleteUsers for ids {Ids}", string.Join(", ", id));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DeleteUsers for ids {Ids}", string.Join(", ", id));
                throw;
            }
        }

        public async Task<bool> DeleteFeedbacks(List<int> id)
        {
            if (id == null || id.Count == 0) return false;

            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., feedback(s) were deleted)
                return (await con.ExecuteAsync("DELETE FROM feedback WHERE id IN @Ids", new { Ids = id }) > 0);
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during DeleteFeedbacks for ids {Ids}", string.Join(", ", id));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DeleteFeedbacks for ids {Ids}", string.Join(", ", id));
                throw;
            }
        }

        public async Task<bool> DeleteEvents(List<int> id)
        {
            if (id == null || id.Count == 0) return false;

            try
            { 
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., event(s) were deleted)
                return (await con.ExecuteAsync("DELETE FROM events WHERE id IN @Ids", new { Ids = id }) > 0);
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during DeleteEvents for ids {Ids}", string.Join(", ", id));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DeleteEvents for ids {Ids}", string.Join(", ", id));
                throw;
            }
        }

        public async Task<bool> DeleteBookings(List<int> id)
        {
            if (id == null || id.Count == 0) return false;

            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                // Return true if at least one row was affected (i.e., booking(s) were deleted)
                return (await con.ExecuteAsync("DELETE FROM bookings WHERE id IN @Ids", new { Ids = id }) > 0);
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during DeleteBookings for ids {Ids}", string.Join(", ", id));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DeleteBookings for ids {Ids}", string.Join(", ", id));
                throw;
            }
        }
    }
}
