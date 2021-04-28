using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Blipper.Services
{
    public class TelegramRelayService
    {
        private static TelegramBotClient Bot;

        private IConfiguration config;

        //
        private static string GangChatId { get; set; } = "-524662757";
        //
        private static string GuysChatId { get; set; } = "-524662757";

        public TelegramRelayService(IConfiguration config)
        {
            this.config = config;
        }

        public void InitService()
        {
            var botId = config.GetValue<string>("TelegramBotId");
            
            Bot = new TelegramBotClient(botId);
            Bot.StartReceiving(new UpdateType[] { UpdateType.Message });
            Bot.OnUpdate += (s, a) => NewTelegramMessageEvent(a);

            TwilioClient.Init(config.GetValue<string>("TwilioConfig:AccountSID"), config.GetValue<string>("TwilioConfig:AuthToken"));
        }

        public void NewTelegramMessageEvent(UpdateEventArgs e)
        {
            var message = "";

            if(e.Update.Type == UpdateType.Message)
                message += e.Update.Message.Chat.Title + ": " +
                    e.Update.Message.From.FirstName + " " +
                    e.Update.Message.From.LastName.Substring(0, 1) + ": " +
                    e.Update.Message.Text;

            SendTwilioMessage(message);
        }

        public async Task IncomingSMSMessageAsync(string message)
        {
            if (message.Length == 0)
                return;
            
            var type = message.Substring(0, 1).ToLower();
            if (type == "e")
                await Bot.SendTextMessageAsync(GangChatId, message.Substring(1, message.Length - 1));
            if (type == "g")
                await Bot.SendTextMessageAsync(GuysChatId, message.Substring(1, message.Length - 1));
        }

        private void SendTwilioMessage(string message)
        {
            var clientNumber = config.GetValue<string>("RelayNumber");
            var from = config.GetValue<string>("TwilioConfig:From");

            MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(from),
                to: new Twilio.Types.PhoneNumber(clientNumber),
                statusCallback: new Uri($"{ config.GetValue<string>("PublicUrl") }/api/sms/status/{ Guid.NewGuid() }")
            ).Wait();
        }
    }
}
