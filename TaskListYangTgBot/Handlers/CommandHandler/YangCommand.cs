using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class YangCommand : Command
    {
        public override string Keyword => "/yang";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            try
            {
                DataContext db = new DataContext(StaticFields.connectionString);
                CommandStatus commandStatus = new CommandStatus();
                Pagination pagination = new Pagination();
                long chatId = TelegramBotHelper.GetChatId(update);
                int typeSorting = db.GetTable<Users>().FirstOrDefault(x => x.Id == chatId).TypeSorting;
                string tokenYang = StaticFields.GetUserToken(chatId);

                commandStatus.AddToDictionaryTaskList(typeSorting, tokenYang, chatId);
                pagination.AddToDictionaryNumberPage(chatId, 0);
                if (CommandStatus.taskListsByChatId[chatId].Count <= 20)
                {
                    await client.SendTextMessageAsync(chatId, "Количество заданий: " + CommandStatus.taskListsByChatId[chatId].Count + "\r\n" + "🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸\r\n");
                    CreateMsgTask(update, tokenYang, CommandStatus.taskListsByChatId[chatId], client);
                }
                else
                {
                    List<Root> pageTasks = pagination.GetPage(CommandStatus.taskListsByChatId[chatId], Pagination.numberOfPageDic[chatId], 20);

                    await client.SendTextMessageAsync(chatId, "Количество заданий: " + CommandStatus.taskListsByChatId[chatId].Count + "\r\n" + "🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸\r\n",
                        replyMarkup: pageTasks.Count >= 20 ? StaticFields.KeyboardForYangCommand : new ReplyKeyboardRemove());
                    CreateMsgTask(update, tokenYang, pageTasks, client);
                }

                db.Dispose();
            }
            catch (Exception ex)
            {
                WriteToDB.WriteLogToDB(TelegramBotHelper.GetChatId(update), update.Message.Text, ex);
                await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Ошибка!");
            }
        }
        public async void CreateMsgTask(Update update, string tokenYang, List<Root> taskList, TelegramBotClient client)
        {
            string message;
            CommandStatus commandStatus = new CommandStatus();
            long chatId = TelegramBotHelper.GetChatId(update);

            if (taskList.Count == 0)
            {
                await client.SendTextMessageAsync(chatId, "Заданий нет", replyMarkup: new ReplyKeyboardRemove());
                Pagination.numberOfPageDic[chatId] = 0;
            }
            else
            {
                foreach (var item in taskList)
                {
                    message = "🔹" + "Задание 🔹\r\n" + item.description + "(" + item.pools[0].reward + ")" + "\r\n" + item.title + "\r\n";

                    if (item.pools[0].activeAssignments != null)
                    {
                        string linkTask = StaticFields.linkTask + item.pools[0].id + "/" + item.pools[0].activeAssignments[0].id;
                        var takeTaskResponse = ParseYang.RequestToApiTakeTask(item.pools[0].id.ToString(), tokenYang);
                        TakeTask(chatId, takeTaskResponse, client);
                        await client.SendTextMessageAsync(chatId, message, replyMarkup: CreateButton.GetButton(item.pools[0].id, "Выйти", "Ссылка на задание", linkTask));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, message, replyMarkup: CreateButton.GetButton(item.pools[0].id, "Приступить", "В любимые"));
                        Thread.Sleep(100);
                    }

                }
            }
        }
        public async void TakeTask(long chatId, RootTakeTaskResponse takeTaskResponse, TelegramBotClient client)
        {
            try
            {
                if (takeTaskResponse.statusCode == 200)
                {
                    if (takeTaskResponse.tasks != null)
                    {
                        string linkTask = StaticFields.linkTask + takeTaskResponse.poolId + "/" + takeTaskResponse.id;
                        string reward = takeTaskResponse.reward;
                        string projectName = takeTaskResponse.tasks[0].input_values.data == null ? takeTaskResponse.title : takeTaskResponse.tasks[0].input_values.data.version_info.project_name;
                        string environmentShort = "";
                        string environment = "";
                        string checkEnvironmentOld = "";
                        string checkEnvironment = "";
                        string urlTestStand = "";
                        IReplyMarkup replyMarkup = null;

                        if (takeTaskResponse.tasks[0].input_values.data != null)
                        {
                            urlTestStand = takeTaskResponse.tasks[0].input_values.data.version_info.test_stend;
                            environmentShort = takeTaskResponse.tasks[0].input_values.data.testrun_info.environment != null ? $"({Regex.Replace(takeTaskResponse.tasks[0].input_values.data.testrun_info.environment, @"<[^>]+>|&nbsp;|&emsp;", " ")})" : "";
                            checkEnvironmentOld = takeTaskResponse.tasks[0].input_values.data.testrun_info.final_requester_code != null ? $"Код проверки окружения: {takeTaskResponse.tasks[0].input_values.data.testrun_info.final_requester_code}" : "";

                            if (takeTaskResponse.tasks[0].input_values.data.testrun_info.env_descr != null)
                            {
                                environment = $"Окружение: {Regex.Replace(takeTaskResponse.tasks[0].input_values.data.testrun_info.env_descr.Replace("unknown", ""), @"<[^>]+>|&nbsp;|&emsp;", " ")}";
                            }
                            else
                            {
                                environment = ParseWebEnvironment(takeTaskResponse);
                            }

                            checkEnvironment = takeTaskResponse.tasks[0].input_values.data.testrun_info.env_requester_code_explanation != null && takeTaskResponse.tasks[0].input_values.data.testrun_info.env_requester_code_explanation.Any() ? takeTaskResponse.tasks[0].input_values.data.testrun_info.env_requester_code_explanation[0] : "";
                        }

                        string message = $"🔹 Взято задание 🔹\r\n{projectName} ({reward})\r\n\r\n{environment}{environmentShort}\r\n{checkEnvironmentOld}";
                        replyMarkup = CreateButton.GetButton(takeTaskResponse, linkTask, checkEnvironment, urlTestStand);
                        await client.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
                        //: checkEnvironment == "" ? GetButton(takeTaskResponse.poolId, "Выйти", "Ссылка на задание", linkTask) : GetButton(takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на проверку окружения", "Ссылка на тестовый стенд", linkTask, checkEnvironment, urlTestStand));
                    }
                }
                else
                {
                    await HandleErrorMessages(chatId, takeTaskResponse, client);
                }
            }
            catch (Exception ex)
            {
                WriteToDB.WriteLogToDB(chatId, "Exception in TakeTask", ex);
                await client.SendTextMessageAsync(chatId, "Ошибка при взятии задания!");
            }
        }

        private static async System.Threading.Tasks.Task HandleErrorMessages(long chatId, RootTakeTaskResponse takeTaskResponse, TelegramBotClient client)
        {
            if (takeTaskResponse.message == "There are no more assignments in current pool")
            {
                await client.SendTextMessageAsync(chatId,
                    "Ошибка! Задание забрали");
            }
            else if (takeTaskResponse.message == "Too many active assignments")
            {
                await client.SendTextMessageAsync(chatId,
                    "Ошибка! Нельзя взять больше заданий");
            }
            else if (takeTaskResponse.message == "There are no more assignments in merged pools")
            {
                await client.SendTextMessageAsync(chatId,
                    "Ошибка! Задание забрали");
            }
            else
            {
                await client.SendTextMessageAsync(chatId,
                    "Ошибка! " + takeTaskResponse.message);
            }
        }

        private static string ParseWebEnvironment(RootTakeTaskResponse takeTaskResponse)
        {
            string environment;
            string json;
            using (var clientHttp = new HttpClient())
            {
                var result = clientHttp.GetAsync(takeTaskResponse.tasks[0].input_values.data.testrun_info.required_envs[0]).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            environment = $"Окружение: {Regex.Replace(Regex.Replace(json, @"<.*?>", " ").Replace("ИЛИ", "").Replace(" И ", "").Replace("&gt;", ">").Replace("&lt;", "<"), @"\s+", " ")}";
            return environment;
        }
    }
}
