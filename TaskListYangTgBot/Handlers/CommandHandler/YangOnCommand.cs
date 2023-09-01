using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Threading;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class YangOnCommand : Command
    {
        public override string Keyword => "/yangon";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            if (CommandStatus.commandStatus[TelegramBotHelper.GetChatId(update)] == false)
            {
                StartYangONCommand(update, false, client);
            }
            else
            {
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Команда /yangon или /yangonfavorite уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
        public async void StartYangONCommand(Update update, bool withFavorite, TelegramBotClient client)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            try
            {
                string tokenYang = StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update));
                YangCommand yangCommand = new YangCommand();
                int waitTime = 2000;
                if (withFavorite == true)
                    await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Команда /yangonfavorite включена", replyMarkup: StaticFields.Keyboard);
                else
                    await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Команда /yangon включена", replyMarkup: StaticFields.Keyboard);
                CommandStatus.commandStatus[TelegramBotHelper.GetChatId(update)] = true;
                List<Root> taskList = null;
                List<FavoriteTasks> listFavoriteTasks = db.GetTable<FavoriteTasks>().Where(c => c.UserId.Contains(TelegramBotHelper.GetChatId(update).ToString())).ToList();
                int typeSorting = db.GetTable<Users>().FirstOrDefault(x => x.Id == TelegramBotHelper.GetChatId(update)).TypeSorting;
                for (int i = 1; i < 10000; i++)
                {
                    taskList = GetTaskList(withFavorite, tokenYang, taskList, listFavoriteTasks, typeSorting);

                    if (CommandStatus.commandStatus[TelegramBotHelper.GetChatId(update)] == false)
                    {
                        break;
                    }
                    else if (i % 500 == 0)
                    {
                        await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Заданий ещё нет");
                    }
                    else if (taskList.Count == 0)
                    {
                        Thread.Sleep(waitTime);
                    }
                    else if (CommandStatus.commandStatus[TelegramBotHelper.GetChatId(update)] == true)
                    {
                        await TakeTask(update, withFavorite, client, tokenYang, yangCommand, waitTime, taskList);
                    }
                }
                db.Dispose();
            }
            catch (Exception ex)
            {
                WriteToDB.WriteLogToDB(TelegramBotHelper.GetChatId(update), update.Message.Text, ex);
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Ошибка!");
            }

        }

        private static async System.Threading.Tasks.Task TakeTask(Update update, bool withFavorite, TelegramBotClient client, string tokenYang, YangCommand yangCommand, int waitTime, List<Root> taskList)
        {
            int numTask = 0;

            foreach (var item in taskList)
            {
                numTask++;

                var takeTaskResponse = ParseYang.RequestToApiTakeTask(item.pools[0].id.ToString(), tokenYang);

                if (takeTaskResponse.statusCode == 200)
                {
                    yangCommand.TakeTask(TelegramBotHelper.GetChatId(update), takeTaskResponse, client);
                    CommandStatus.commandStatus[TelegramBotHelper.GetChatId(update)] = false;
                    await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), withFavorite == true ? "Команда /yangonfavorite выключена" : "Команда /yangon выключена", replyMarkup: new ReplyKeyboardRemove());
                    break;
                }
            }
            Thread.Sleep(waitTime);
        }

        private static List<Root> GetTaskList(bool withFavorite, string tokenYang, List<Root> taskList, List<FavoriteTasks> listFavoriteTasks, int typeSorting)
        {
            if (withFavorite)
            {
                var favoriteTasksFilter = ParseYang.RequestToApiTaskList(tokenYang)
                    .Where(x => listFavoriteTasks.Any(q => x.description.Contains(q.FavoriteTask) || x.title.Contains(q.FavoriteTask)));

                taskList = ApplySortingAndFilters(favoriteTasksFilter, typeSorting);
            }
            else
            {
                var defaultTasksFilter = ParseYang.RequestToApiTaskList(tokenYang)
                    .Where(x => x.projectMetaInfo.ignored != true && x.pools[0].activeAssignments == null);

                taskList = ApplySortingAndFilters(defaultTasksFilter, typeSorting);
            }

            return taskList;
        }
        private static List<Root> ApplySortingAndFilters(IEnumerable<Root> tasks, int typeSorting)
        {
            switch (typeSorting)
            {
                case 1:
                    return tasks.OrderBy(r => double.Parse(r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
                case 0:
                    return tasks.OrderByDescending(r => double.Parse(r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
                default:
                    return tasks.ToList();
            }
        }
    }
}
