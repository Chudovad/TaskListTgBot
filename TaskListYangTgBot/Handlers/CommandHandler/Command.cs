using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    abstract class Command
    {
        public abstract string Keyword { get; }

        public abstract void Execute(Update update, TelegramBotClient client);

    }
}
