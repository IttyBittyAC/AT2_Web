using Microsoft.Data.Sqlite;
using BCrypt.Net;
using Dapper;
using MVCApplication.Models;
using System.Reflection.Metadata.Ecma335;

namespace MVCApplication.Data
{
    public class AppDb
    {
        //create tables for db sets
        private readonly string _conn;
        public AppDb(IConfiguration cfg) => _conn = cfg.GetConnectionString("Default") ?? "Data Source=app.db";
        public async Task<User?> Login(string password, string email)
        {
            using var c = new SqliteConnection(_conn);
            var user = await c.QueryFirstOrDefaultAsync(
                "select * from users where email = @email",
                new { email }
                );
            return user is null ?  null : BCrypt.Net.BCrypt.Verify(password, (string)user.password_hash) ? user : null;
        }
        public async Task<User?> Register(string password, string email, string username, string fullname, string role)
        {
            using var c = new SqliteConnection(_conn);

            var UserExist = await c.ExecuteScalarAsync(
                "select count(1) from users where email =@email",
                new { email }
                );
            if (UserExist is not null) return null;

            var id = await c.ExecuteScalarAsync<int>(@"
                insert into users (email, password_hash, username, fullname, role) values (@email, @hash, @username, @fullname, @role) returning id", new { email, hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password), username, fullname, role });
            return await c.QueryFirstOrDefaultAsync(
                "select * from users where id = @id", new {id});
        }
        public async Task EnsureCreated()
        {
            using var c = new SqliteConnection(_conn);
            await c.ExecuteAsync(@"
            create table if not exists users (
                id            integer primary key autoincrement,
                email         text unique not null,
                password_hash text not null,
                username      text not null,
                fullname      text not null,
                role          text not null default 'user',
                created_at    datetime default current_timestamp
            );
            create table if not exists feedback (
                id        integer primary key autoincrement,
                fullname  text not null,
                email     text not null,
                message   text not null,
                date      datetime default current_timestamp
            );
        ");
        }



    }

}
