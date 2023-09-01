using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class FavoritetasksCommand : Command
    {
        public override string Keyword => "/favoritetasks";
        public async override void Execute(Update update, TelegramBotClient client)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            List<FavoriteTasks> favoriteTasks = db.GetTable<FavoriteTasks>().Where(c => c.UserId == TelegramBotHelper.GetChatId(update).ToString()).ToList();
            if (favoriteTasks.Count != 0)
            {
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), StaticFields.RemoveMsg, replyMarkup: CreateButton.GetButton(favoriteTasks));
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), StaticFields.RemoveMsg + StaticFields.FavoriteTaskMsg
                    , replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название задания" });//, replyMarkup: keyboard
            }
            else
            {
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Нет любимых заданий.");
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), StaticFields.FavoriteTaskMsg, replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название задания" });
            }
            db.Dispose();
        }
    }
}
