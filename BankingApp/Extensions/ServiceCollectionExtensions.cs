using BankingApp.Configurations;
using BankingApp.Models;
using BankingApp.Repositories;
using BankingApp.Repositories.Account;
using BankingApp.Services.Account;
using BankingApp.Services.Funds;
using BankingApp.Services.Queue;
using BankingApp.Services.Retry;
using BankingApp.Services.Transaction;
using BankingApp.Validation;
using FluentValidation;

namespace BankingApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBankingAppDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(EfRepository<>));

            services.AddHostedService<AppDbMigrationHandler>();

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ITransactionService, TransactionService>();

            services.AddTransient<IAccountNumberGenerator, AccountNumberGenerator>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IFundsService, FundsService>();

            services.AddSingleton<IOperationQueue, ChannelOperationQueue>();

            services.AddSingleton<IRetryService, RetryService>();

            services.AddScoped<IValidator<DepositTransferModel>, DepositTransferModelValidator>();
            return services;
        }
    }
}