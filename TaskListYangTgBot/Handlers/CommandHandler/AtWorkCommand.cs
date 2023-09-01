using System.Collections.Generic;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class AtWorkCommand : Command
    {
        public override string Keyword => "/atwork";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            YangCommand yangCommand = new YangCommand();
            List<Root> taskListInProgress = ParseYang.RequestToApiTaskList(StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update))).Where(x => x.pools[0].activeAssignments != null && x.pools[0].activeAssignments.Count > 0).ToList();
            if (taskListInProgress.Count != 0)
            {
                foreach (var item in taskListInProgress)
                {
                    var takeTaskResponse = ParseYang.RequestToApiTakeTask(item.pools[0].id.ToString(), StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update)));
                    yangCommand.TakeTask(TelegramBotHelper.GetChatId(update), takeTaskResponse, client);
                }
            }
            else
            {
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Заданий в работе нет");
            }
        }
    }
}
