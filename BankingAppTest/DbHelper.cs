using BankingApp.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BankingAppTest
{
    public static class DbHelper
    {
        public static AppDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}