using System.Collections.Generic;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CallbackHandler
{
    internal class QuitCallback : Callback
    {
        public override string Keyword => "Выйти";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            string tokenYang = StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update));
            List<Root> taskList = ParseYang.RequestToApiTaskList(tokenYang);
            List<Root> leaveTask = taskList.Where(x => x.pools[0].activeAssignments != null && x.pools[0].activeAssignments.Count > 0).ToList();
            ParseYang.RequestToApiLeaveTask(leaveTask[0].pools[0].activeAssignments[0].id, tokenYang);
            await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Вышли");
            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update),
                    "Вышли из задания " + leaveTask[0].description);
        }
    }
}
