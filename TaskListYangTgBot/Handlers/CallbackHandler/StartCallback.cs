using TaskListYangTgBot.Handlers.CommandHandler;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CallbackHandler
{
    internal class StartCallback : Callback
    {
        public override string Keyword => "Приступить";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            YangCommand yangCommand = new YangCommand();
            var takeTaskResponse = ParseYang.RequestToApiTakeTask(update.CallbackQuery.Data.Replace(Keyword, ""), StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update)));
            yangCommand.TakeTask(TelegramBotHelper.GetChatId(update), takeTaskResponse, client);
        }
    }
}
