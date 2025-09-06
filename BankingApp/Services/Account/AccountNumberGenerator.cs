using BankingApp.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankingApp.Services.Account
{
    public class AccountNumberGenerator(AppDbContext db) : IAccountNumberGenerator
    {
        public async Task<string> GenerateAsync()
        {
            var conn = db.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT nextval('account_number_seq')";
            var value = (long)(await cmd.ExecuteScalarAsync())!;
            return $"ACC{value:D6}";
        }
    }
}