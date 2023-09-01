using System;
using System.Data.Linq;
using System.Linq;
using TaskListYangTgBot.Handlers.CommandHandler;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CallbackHandler
{
    internal class FavoriteTaskCallback : Callback
    {
        public override string Keyword => "FavoriteTask";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            long chatId = TelegramBotHelper.GetChatId(update);
            var task = db.GetTable<FavoriteTasks>().FirstOrDefault(x => Convert.ToInt64(x.UserId) == chatId && update.CallbackQuery.Data.Replace("FavoriteTask", "") == x.PoolId);

            db.GetTable<FavoriteTasks>().DeleteOnSubmit(task);
            db.SubmitChanges();
            await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Удалил: " + task.FavoriteTask);
            await client.DeleteMessageAsync(chatId, messageId: update.CallbackQuery.Message.MessageId);
            await client.DeleteMessageAsync(chatId, messageId: update.CallbackQuery.Message.MessageId + 1);
            FavoritetasksCommand favoriteTasks = new FavoritetasksCommand();
            favoriteTasks.Execute(update, client);
        }
    }
}
