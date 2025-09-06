using AutoMapper;
using BankingApp.Entities;
using BankingApp.Exceptions;
using BankingApp.Models;
using BankingApp.Repositories.Account;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Services.Account
{
    public class AccountService(IAccountRepository accountRepository, IAccountNumberGenerator accountNumberGenerator, IMapper mapper) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IAccountNumberGenerator _accountNumberGenerator = accountNumberGenerator;
        private readonly IMapper _mapper = mapper;

        public async Task<GetAccountDto> CreateAsync(CreateAccountDto createAccountDto)
        {
            var entity = _mapper.Map<AccountEntity>(createAccountDto);
            entity.AccountNumber = await _accountNumberGenerator.GenerateAsync();
            await _accountRepository.CreateAsync(entity);
            await _accountRepository.SaveChangesAsync();
            return _mapper.Map<GetAccountDto>(entity);
        }

        public async Task<List<GetAccountDto>> GetAllAsync()
        {
            var accounts = await _accountRepository.AsQueryable().OrderBy(ac => ac.AccountNumber).ToListAsync();
            return _mapper.Map<List<GetAccountDto>>(accounts);
        }

        public async Task<GetAccountDto?> FindByAccountNumberAsync(string accountNumber)
        {
            var account = await _accountRepository.FindByAccountNumberAsync(accountNumber);
            return account == null ? null : _mapper.Map<GetAccountDto>(account);
        }

        public async Task UpdateByAccountNumberAsync(UpdateAccountDto updateAccountDto)
        {
            var account = await _accountRepository.FindTrackedByAccountNumberAsync(updateAccountDto.AccountNumber)
                ?? throw new AccountNotFoundException("Account not found.");

            account.Balance = updateAccountDto.Balance;

            await _accountRepository.SaveChangesAsync();
        }
    }
}