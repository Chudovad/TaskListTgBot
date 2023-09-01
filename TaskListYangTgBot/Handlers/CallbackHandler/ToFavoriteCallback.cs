using System;
using System.Data.Linq;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CallbackHandler
{
    internal class ToFavoriteCallback : Callback
    {
        public override string Keyword => "В любимые";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            CommandStatus commandStatus = new CommandStatus();
            long chatId = TelegramBotHelper.GetChatId(update);
            commandStatus.AddToDictionaryTaskList(db.GetTable<Users>().FirstOrDefault(x => x.Id == chatId).TypeSorting, StaticFields.GetUserToken(chatId), chatId);
            var task = CommandStatus.taskListsByChatId[chatId].Where(x => x.pools[0].id == Convert.ToInt32(update.CallbackQuery.Data.Replace("В любимые", ""))).ToList();
            FavoriteTasks favoriteTasks = new FavoriteTasks
            {
                UserId = chatId.ToString(),
                FavoriteTask = task[0].description,
                PoolId = task[0].pools[0].id.ToString(),
            };

            db.GetTable<FavoriteTasks>().InsertOnSubmit(favoriteTasks);
            db.SubmitChanges();
            await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Добавил в любимые: " + task[0].description);
            await client.SendTextMessageAsync(chatId, "Добавили задание " + task[0].description + " в любимые");
        }
    }
}
