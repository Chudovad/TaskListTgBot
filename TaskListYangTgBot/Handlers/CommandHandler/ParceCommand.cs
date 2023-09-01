using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types;
using Telegram.Bot;
using TaskListYangTgBot.Services;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class ParseCommand : Command
    {
        public override string Keyword => "/parse";
        public async override void Execute(Update update, TelegramBotClient client)
        {

            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "");
        }
    }
}
