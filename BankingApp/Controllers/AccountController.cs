using BankingApp.Models;
using BankingApp.Services.Account;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var accounts = await _accountService.GetAllAsync();
        return Ok(accounts);
    }

    [ActionName(nameof(GetByAccountNumberAsync))]
    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> GetByAccountNumberAsync(string accountNumber)
    {
        var account = await _accountService.FindByAccountNumberAsync(accountNumber);
        return account == null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateAccountDto createAccountDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdAccount = await _accountService.CreateAsync(createAccountDto);

        return CreatedAtAction(
            nameof(GetByAccountNumberAsync),
            new { accountNumber = createdAccount.AccountNumber },
            createdAccount);
    }
}