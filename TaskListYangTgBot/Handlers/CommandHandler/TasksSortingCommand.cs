using System.Data.Linq;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class TasksSortingCommand : Command
    {
        public override string Keyword => "/tasks_sorting";
        public async override void Execute(Update update, TelegramBotClient client)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            int typeSorting = db.GetTable<Users>().FirstOrDefault(x => x.Id == TelegramBotHelper.GetChatId(update)).TypeSorting;
            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), StaticFields.GetTaskSortingText(typeSorting), replyMarkup: CreateButton.GetButton(StaticFields.TypesSorting));
            db.Dispose();
        }
    }
}
