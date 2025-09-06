using BankingApp.Models;
using BankingApp.Services.Account;
using BankingApp.Services.Funds;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/funds")]
    public class FundsController(IFundsService fundsService, IOperationQueue operationQueue, IValidator<DepositTransferModel> validator) : ControllerBase
    {
        private readonly IFundsService _fundsService = fundsService;
        private readonly IOperationQueue _operationQueue = operationQueue;
        private readonly IValidator<DepositTransferModel> _validator = validator;

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositModel model)
        {
            await _operationQueue.EnqueueAsync(() => _fundsService.DepositAsync(model));
            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] DepositModel model)
        {
            await _operationQueue.EnqueueAsync(() => _fundsService.WithdrawAsync(model));
            return Ok();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] DepositTransferModel model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _operationQueue.EnqueueAsync(() => _fundsService.TransferAsync(model));
            return Ok();
        }
    }
}