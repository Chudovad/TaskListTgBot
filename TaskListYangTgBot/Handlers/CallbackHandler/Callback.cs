using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CallbackHandler
{
    abstract class Callback
    {
        public abstract string Keyword { get; }

        public abstract void Execute(Update update, TelegramBotClient client);

    }
}
