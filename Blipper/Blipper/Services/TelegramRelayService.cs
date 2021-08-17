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

        private static string GangChatId { get; set; }
        private static string GuysChatId { get; set; }
        private static string TestChatId { get; set; }

        public List<(long, DateTime)> Messages { get; set; } = new List<(long, DateTime)>();

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

            GangChatId = config.GetValue<string>("Chats:Gang");
            GuysChatId = config.GetValue<string>("Chats:Guys");
            TestChatId = config.GetValue<string>("Chats:Test");

            TwilioClient.Init(config.GetValue<string>("TwilioConfig:AccountSID"), config.GetValue<string>("TwilioConfig:AuthToken"));
        }

        public void NewTelegramMessageEvent(UpdateEventArgs e)
        {
            var message = "";

            if (e.Update.Type == UpdateType.Message && e.Update.Message.Type == MessageType.Text && e.Update.Message.Text.Trim().Length > 0)
            {
                message += e.Update.Message.Chat.Title + ": " +
                    e.Update.Message.From.FirstName +
                    (e.Update.Message.From.LastName != null ? " " + e.Update.Message.From.LastName.Substring(0, 1) : "") + ": " +
                    e.Update.Message.Text;

                Messages.Add((e.Update.Message.Chat.Id, DateTime.Now));

                SendTwilioMessage(message);
            }
        }

        public async Task IncomingSMSMessageAsync(string message)
        {
            if (message.Length == 0)
                return;

            var type = message.Substring(0, 2).ToLower();
            message = message.Substring(2, message.Length - 2).Trim();

            var chatId = "";
            switch (type)
            {
                case "e.":
                    chatId = GangChatId;
                    break;
                case "g.":
                    chatId = GuysChatId;
                    break;
                case "t.":
                    chatId = TestChatId;
                    break;
                default:
                    var lastMsg = Messages.OrderByDescending(m => m.Item2).FirstOrDefault();
                    await Bot.SendTextMessageAsync(lastMsg.Item1, type + message);
                    break;
            }

            if(!string.IsNullOrEmpty(chatId))
                await Bot.SendTextMessageAsync(chatId, message);
            Console.WriteLine(chatId);
            Messages.Add((long.Parse(chatId), DateTime.Now));
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
