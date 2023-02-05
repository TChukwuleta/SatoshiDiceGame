using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.Transactions.Commands;
using SatoshiDice.Application.Transactions.Queries;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly string accessToken;
        public TransactionController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
        }

        [HttpPost("createtransaction")]
        public async Task<ActionResult<Result>> CreateTransaction(CreateTransactionCommand command)
        {
            try
            {
                return await Mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to create transaction. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("gettransactionsbyid/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllTransactionsByUser(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetAllTransactionsQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetTransactionById(int id, string userid)
        {
            try
            {
                return await Mediator.Send(new GetTransactionByIdQuery { Id = id, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transaction retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbytxnid/{txnid}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetTransactionByTxnId(string txnid, string userid)
        {
            try
            {
                return await Mediator.Send(new GetTransactionsByTxnIdQuery { TxnId = txnid, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transaction retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallcredit/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllDebitTransactionsByUser(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetCreditTransactionsByUserIdQuery { UserId = userid, Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Credit transactions retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getalldebit/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllCreditTransactionsByUser(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetDebitTransactionsByUserIdQuery { UserId = userid,  Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Debit transactions retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
