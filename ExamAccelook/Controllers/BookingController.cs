using ExamAccelook.Contracts.RequestModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamAccelook.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<PostBookTicketRequest> _postBookTicketValidator;
        private readonly IValidator <GetBookedTicketRequest> _getBookedTicketValidator;
        private readonly IValidator<EditBookedTicketRequest> _editBookedTicketValidator;

        public BookingController(
            IMediator mediator, 
            IValidator<PostBookTicketRequest> postBookTicketValidator, 
            IValidator<GetBookedTicketRequest> getBookedTicketValidator,
            IValidator<EditBookedTicketRequest> editBookedTicketValidator
        )
        {
            _mediator = mediator;
            _postBookTicketValidator = postBookTicketValidator;
            _getBookedTicketValidator = getBookedTicketValidator;
            _editBookedTicketValidator = editBookedTicketValidator;

        }

        // POST api/v1/book-ticket
        [HttpPost("book-ticket")]
        public async Task<IActionResult> PostBookTicket([FromBody] PostBookTicketRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _postBookTicketValidator.ValidateAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("Failed to validate data");

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        // GET api/v1/get-booked-ticket/{BookedTicketId}
        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> GetBookedTicket([FromRoute] int BookedTicketId, CancellationToken cancellationToken)
        {
            var request = new GetBookedTicketRequest { BookedTicketId = BookedTicketId };

            var validationResult = await _getBookedTicketValidator.ValidateAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("Failed to validate data");

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        // PUT api/v1/edit-booked-ticket/{BookedTicketId}
        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> PutEditBookedTicket([FromRoute] int BookedTicketId, [FromBody] EditBookedTicketRequest request, CancellationToken cancellationToken)
        {
            if (request == null) request = new EditBookedTicketRequest();

            request.BookedTicketId = BookedTicketId;

            var validationResult = await _editBookedTicketValidator.ValidateAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("Failed to validate data");

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}
