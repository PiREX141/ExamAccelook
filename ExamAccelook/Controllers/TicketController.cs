using ExamAccelook.Contracts.RequestModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamAccelook.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<GetAvailableTicketRequest> _getAvailableTicketValidator;
        private readonly IValidator<RevokeTicketRequest> _revokeTicketValidator;

        public TicketController(IMediator mediator, IValidator<GetAvailableTicketRequest> getAvailableTicketValidator, IValidator<RevokeTicketRequest> revokeTicketValidator)
        {
            _mediator = mediator;
            _getAvailableTicketValidator = getAvailableTicketValidator;
            _revokeTicketValidator = revokeTicketValidator;
        }

        // GET: api/<TicketController>
        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets
        (
            [FromQuery] GetAvailableTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var validationResult = await _getAvailableTicketValidator.ValidateAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("Failed to validate data");

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(ModelState);
            }

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        // DELETE api/v1/revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}
        [HttpDelete("revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}")]
        public async Task<IActionResult> RevokeTicket([FromRoute] int BookedTicketId, [FromRoute] string TicketCode, [FromRoute(Name = "Qty")] int Quantity, CancellationToken cancellationToken)
        {
            var request = new RevokeTicketRequest
            {
                BookedTicketId = BookedTicketId,
                TicketCode = TicketCode,
                Quantity = Quantity
            };

            var validationResult = await _revokeTicketValidator.ValidateAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("Failed to validate data");

            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails();

                foreach (var error in validationResult.Errors)
                {
                    problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                }

                problemDetails.Title = "One or more validation errors occurred.";
                problemDetails.Status = 400;
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7807";

                return BadRequest(problemDetails);
            }

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}
