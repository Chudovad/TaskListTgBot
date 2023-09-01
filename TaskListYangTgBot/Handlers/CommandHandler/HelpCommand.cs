using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class HelpCommand : Command
    {
        public override string Keyword => "/help";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), StaticFields.HelpMsg, parseMode: ParseMode.MarkdownV2);
        }
    }
}
