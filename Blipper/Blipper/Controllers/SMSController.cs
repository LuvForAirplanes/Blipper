using Blipper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;

namespace Blipper
{
    [Route("api/[controller]")]
    public class SMSController : TwilioController
    {
        private readonly TelegramRelayService relayService;

        public SMSController(TelegramRelayService relayService)
        {
            this.relayService = relayService;
        }

        [AllowAnonymous]
        [HttpPost("status/{id}")]
        public async Task<IActionResult> StatusUpdateAsync(string id, SmsStatusCallbackRequest status)
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("incoming")]
        public async Task<IActionResult> IncomingMessageAsync(SmsRequest incomingMessage)
        {
            await relayService.IncomingSMSMessageAsync(incomingMessage.Body);
            return Ok();
        }
    }
}
