using System.Net;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Chat.ApplicationServices.ChatModule.Abstract;
using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Order.API.Controllers
{
    [Authorize]
    [Route("api/chat/conversation")]
    [ApiController]
    public class ConversationController : ApiControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(
            ILogger<ConversationController> logger,
            IConversationService conversationService
        )
            : base(logger)
        {
            _conversationService = conversationService;
        }

        [HttpGet("find-all")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserTableAccountManager)]
        public async Task<ApiResponse> FindAll(PagingConversationDto input)
        {
            try
            {
                return new(await _conversationService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("find-by-id/{id}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public async Task<ApiResponse> FindById(string id)
        {
            try
            {
                return new(await _conversationService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ApiResponse> Create(List<string> input)
        {
            try
            {
                return new(await _conversationService.Create(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPut("delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserButtonAccountManagerDelete)]
        public ApiResponse Delete(int id)
        {
            try
            {
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
