using backend_dotnet.Core.Constants;
using backend_dotnet.Core.Dtos.Message;
using backend_dotnet.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        //Route -> Create a new message to send to another user
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto request)
        {
            var result = await _messageService.CreateNewMessageAsync(User, request);
            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
            return StatusCode(result.StatusCode, result.Message);
        }


        //Route -> Get All messages for the current user, Either sent or received
        [HttpGet("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMyMessages()
        {
            var result = await _messageService.GetMyMessagesAsync(User);
            return Ok(result);
        }


        //Route -> Get All messages with Owner Access and Admin Access
        [HttpGet]
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMessagesAsync()
        {
            var result = await _messageService.GetMessagesAsync();
            return Ok(result);
        }
    }
}
