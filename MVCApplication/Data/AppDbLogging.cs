using Dapper;
using Microsoft.Data.Sqlite;
using MVCApplication.Models;

namespace MVCApplication.Data
{
    public partial class AppDb
    {
        /// <summary>
        /// Saves a log entry to the database.
        /// </summary>
        /// <param name="log">The log entry being saved.</param>
        /// <returns>True if the log was saved successfully, otherwise false.</returns>
        public async Task<bool> SaveLog(Log log)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                int rowsAffected = await con.ExecuteAsync(@"
                    INSERT INTO logs 
                    (
                        IsError,
                        UserName,
                        Role,
                        View,
                        Message,
                        DateTime
                    )
                    VALUES 
                    (
                        @IsError,
                        @UserName,
                        @Role,
                        @View,
                        @Message,
                        @DateTime
                    )", log);

                return rowsAffected > 0;
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during SaveLog for view {View}", log.View);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during SaveLog for view {View}", log.View);
                throw;
            }
        }

        /// <summary>
        /// Gets all application logs from the database.
        /// </summary>
        /// <returns>A list of logs, or null if there was a database error.</returns>
        public async Task<List<Log>?> GetLogs()
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(_conn);

                return (await con.QueryAsync<Log>(
                    "SELECT * FROM logs ORDER BY DateTime DESC")).ToList();
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Database error during GetLogs");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during GetLogs");
                throw;
            }
        }
    }
}