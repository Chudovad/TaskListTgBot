using System;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using TaskListYangTgBot.Handlers.CallbackHandler;
using TaskListYangTgBot.Handlers.CommandHandler;
using TaskListYangTgBot.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangTgBot.Services
{
    internal class TelegramBotHelper
    {
        TelegramBotClient client;
        static CancellationTokenSource cts = null;


        public void GetUpdate()
        {
            client = new TelegramBotClient(StaticFields.tokenBot);
            cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            client.StartReceiving(HandleUpdateAsyns, HandleErrorAsyns, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
        private static async System.Threading.Tasks.Task HandleErrorAsyns(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message} {DateTime.Now.AddHours(StaticFields.countAddHours)}",
                _ => exception.ToString()
            };

            WriteToDB.WriteMsgToDB(ErrorMessage, 0);
            Console.WriteLine(ErrorMessage);
            cts.Cancel();
            cts.Dispose();
            await System.Threading.Tasks.Task.Delay(3000);
            Thread.Sleep(6000);
            TelegramBotHelper telegramBotHelper = new TelegramBotHelper();
            telegramBotHelper.GetUpdate();
        }

        private async System.Threading.Tasks.Task HandleUpdateAsyns(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            DataContext db = new DataContext(StaticFields.connectionString);

            if (update.Type == UpdateType.Message)
            {
                try
                {
                    if (update.Message.Text != "/start")
                    {
                        Users users = db.GetTable<Users>().FirstOrDefault(x => x.Id == GetChatId(update));
                        users.FirstName = update.Message.Chat.FirstName;
                        users.LastName = update.Message.Chat.LastName;
                        users.UserName = update.Message.Chat.Username;
                        db.SubmitChanges();
                    }
                    WriteToDB.WriteMsgToDB(update.Message.Text, GetChatId(update));
                    AnswerMessages(update);
                }
                catch (Exception ex)
                {
                    WriteToDB.WriteLogToDB(GetChatId(update), update.Message.Text, ex);
                    await client.SendTextMessageAsync(GetChatId(update), "Ошибка!");
                }

            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                try
                {
                    Users users = db.GetTable<Users>().FirstOrDefault(x => x.Id == GetChatId(update));
                    users.FirstName = update.CallbackQuery.Message.Chat.FirstName;
                    users.LastName = update.CallbackQuery.Message.Chat.LastName;
                    users.UserName = update.CallbackQuery.Message.Chat.Username;
                    db.SubmitChanges();
                    CommandStatus commandStatus = new CommandStatus();
                    YangCommand yangCommand = new YangCommand();

                    WriteToDB.WriteMsgToDB(update.CallbackQuery.Data, GetChatId(update));

                    Callback[] callbackMessage =
                    {
                        new QuitCallback(),
                        new StartCallback(),
                        new FavoriteTaskCallback(),
                        new ToFavoriteCallback(),
                    };
                    Callback callback = callbackMessage.FirstOrDefault(c => update.CallbackQuery.Data.Contains(c.Keyword));

                    if (callback != null)
                    {
                        callback.Execute(update, (TelegramBotClient)client);
                    }

                    if (StaticFields.TypesSorting.Any(x => x.Contains(update.CallbackQuery.Data)))
                    {
                        Users user = db.GetTable<Users>().FirstOrDefault(x => x.Id == GetChatId(update));
                        user.TypeSorting = StaticFields.TypesSorting.FindIndex(x => x.Contains(update.CallbackQuery.Data));
                        db.SubmitChanges();
                        await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");
                        await client.SendTextMessageAsync(GetChatId(update), $"Выбрана сортировка: {update.CallbackQuery.Data}");
                    }
                }
                catch (Exception ex)
                {
                    WriteToDB.WriteLogToDB(GetChatId(update), update.CallbackQuery.Data, ex);
                    await client.SendTextMessageAsync(GetChatId(update), "Ошибка!");
                }
            }
            else if (update.Type == UpdateType.PreCheckoutQuery)
            {
                await ReceiptOfPayment(client, update, db, cancellationToken);
            }
        }

        private static async System.Threading.Tasks.Task ReceiptOfPayment(ITelegramBotClient client, Update update, DataContext db, CancellationToken cancellationToken)
        {
            await client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id, cancellationToken: cancellationToken);
            client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).Wait(TimeSpan.FromSeconds(10));
            Console.WriteLine("Status: " + client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).Status
                + " IsCompleted: " + client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).IsCompleted
                + " IsCanceled: " + client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).IsCanceled
                + " IsCompletedSuccessfully: " + client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).IsCompletedSuccessfully
                + " Exception: " + client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).Exception
                );

            //if (client.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id).IsCompleted == true)
            //{
            int.TryParse(string.Join("", Convert.ToString(update.PreCheckoutQuery.From).Where(c => char.IsDigit(c))), out int userId);
            Console.WriteLine(update.PreCheckoutQuery.From + "ID: " + update.PreCheckoutQuery.Id + "Id user: " + userId);
            Payment payment = new Payment
            {
                UserId = userId,
                UserInfo = Convert.ToString(update.PreCheckoutQuery.From),
                PaymentId = update.PreCheckoutQuery.Id
            };

            db.GetTable<Payment>().InsertOnSubmit(payment);
            db.SubmitChanges();

            Users user = db.GetTable<Users>().FirstOrDefault(x => x.Id == userId);
            user.Payment = true;
            db.SubmitChanges();

            //}         
        }

        private async void Command(Update update)
        {
            var msg = update.Message;
            try
            {
                string message = "";
                ActiveCount checkToken = ParseYang.RequestToApiCheckToken(StaticFields.GetUserToken(GetChatId(update)));
                CommandStatus commandStatus = new CommandStatus();
                YangCommand yangCommand = new YangCommand();
                YangOnCommand yangonCommand = new YangOnCommand();

                if (!CommandStatus.commandStatus.ContainsKey(GetChatId(update)))
                {
                    CommandStatus.commandStatus.Add(GetChatId(update), false);
                }

                //Пример использования полиморфизма или абстракции)
                Command[] commands = new Command[]
                {
                    new StartCommand(),
                    new HelpCommand(),
                    new PayCommand(),
                    new FavoritetasksCommand(),
                    new TasksSortingCommand(),
                };
                Command userCommand = commands.FirstOrDefault(c => c.Keyword == msg.Text);
                if (userCommand != null)
                {
                    userCommand.Execute(update, client);
                    return;
                }
                Command[] commandsWithTokenCheck = new Command[]
                {
                    new NormaCommand(),
                    new YangCommand(),
                    new YangOnCommand(),
                    new YangOnFavoriteCommand(),
                    new AtWorkCommand(),
                };
                Command userCommandWithTokenCheck = commandsWithTokenCheck.FirstOrDefault(c => c.Keyword == msg.Text);

                if (userCommandWithTokenCheck != null && checkToken.message == null)
                {
                    userCommandWithTokenCheck.Execute(update, client);
                }
                else if (StaticFields.GetUserToken(GetChatId(update)).Length == 0)
                {
                    message = "Пришли свой OAuth токен Янга и бот будет работать\\. \r" + StaticFields.LinkToManual;
                    await client.SendTextMessageAsync(GetChatId(update), message, parseMode: ParseMode.MarkdownV2);
                }
                else
                {
                    await client.SendTextMessageAsync(GetChatId(update), "Команда не найдена");
                }
            }
            catch (Exception ex)
            {
                WriteToDB.WriteLogToDB(GetChatId(update), update.Message.Text, ex);
                await client.SendTextMessageAsync(GetChatId(update), "Ошибка!");
            }
        }


        private async void AnswerMessages(Update update)
        {
            var msg = update.Message;
            DataContext db = new DataContext(StaticFields.connectionString);
            Table<Users> users = db.GetTable<Users>();
            var daysOfUse = new TimeSpan(1, 1, 1, 1);
            string tokenYang = StaticFields.GetUserToken(GetChatId(update));
            ActiveCount checkToken = ParseYang.RequestToApiCheckToken(tokenYang);
            Pagination pagination = new Pagination();
            YangCommand yangCommand = new YangCommand();

            if (msg.Text != "/start")
            {
                daysOfUse = DateTime.Now.AddHours(StaticFields.countAddHours) - users.SingleOrDefault(row => row.Id == GetChatId(update)).DateReg;
            }
            if (msg.Type == MessageType.Video || msg.Type == MessageType.Photo)
            {

            }
            else if (msg.Text == null)
            {
                await client.SendTextMessageAsync(GetChatId(update), StaticFields.HelpMsg, replyToMessageId: msg.MessageId, parseMode: ParseMode.MarkdownV2);
            }
            else if (!msg.Text.Contains("/pay") && !msg.Text.Contains("/help") && !msg.Text.Contains("/start") && users.SingleOrDefault(row => row.Id == GetChatId(update)).Payment == false && daysOfUse.Days > 7)
            {
                await client.SendTextMessageAsync(GetChatId(update), "Для продолжения использования бота надо оплатить. Нажмите на команду /pay");
            }
            else if (msg.Text != null && !string.IsNullOrEmpty(msg.Text) && msg.Text.Contains("AQAD"))
            {
                await ValidationOfYangToken(update, msg, db, users);
            }
            else if (msg.Text != null && !string.IsNullOrEmpty(msg.Text) && Uri.IsWellFormedUriString(msg.Text, UriKind.Absolute))
            {
                await client.SendTextMessageAsync(GetChatId(update), $"`{msg.Text}`", replyToMessageId: msg.MessageId, parseMode: ParseMode.MarkdownV2);
            }
            else if (msg.Text[0] == '/')
            {
                Command(update);
            }
            else if (msg.Text == "Завершить команду")
            {
                CommandStatus.commandStatus[GetChatId(update)] = false;
                await client.SendTextMessageAsync(GetChatId(update), "Команда выключена", replyMarkup: new ReplyKeyboardRemove());
            }
            else if (msg.Text == "Ещё 20 заданий")
            {
                Pagination.numberOfPageDic[GetChatId(update)]++;
                yangCommand.CreateMsgTask(update, tokenYang, pagination.GetPage(CommandStatus.taskListsByChatId[GetChatId(update)], Pagination.numberOfPageDic[GetChatId(update)], 20), client);
            }
            else if (msg.Text == "Ещё 50 заданий")
            {
                Pagination.numberOfPageDic[GetChatId(update)]++;
                yangCommand.CreateMsgTask(update, tokenYang, pagination.GetPage(CommandStatus.taskListsByChatId[GetChatId(update)], Pagination.numberOfPageDic[GetChatId(update)], 50), client);
            }
            else if (msg.Text == "Завершить")
            {
                Pagination.numberOfPageDic[GetChatId(update)] = 0;
                await client.SendTextMessageAsync(GetChatId(update), "Команда выключена", replyMarkup: new ReplyKeyboardRemove());
            }
            else if (msg.ReplyToMessage != null && msg.ReplyToMessage.Text.Contains("название любимого задания"))
            {
                await AddFavoriteTask(update, msg, db);
            }
            else if (checkToken.message != null)
            {
                await client.SendTextMessageAsync(GetChatId(update), "Пришли свой токен. Помощь /help", replyToMessageId: msg.MessageId);
            }
            else
            {
                await client.SendTextMessageAsync(GetChatId(update), StaticFields.CommandMsg, replyToMessageId: msg.MessageId, parseMode: ParseMode.MarkdownV2);
            }
            db.Dispose();
        }

        private async System.Threading.Tasks.Task AddFavoriteTask(Update update, Message msg, DataContext db)
        {
            FavoriteTasks favoriteTasks = new FavoriteTasks
            {
                UserId = GetChatId(update).ToString(),
                FavoriteTask = msg.Text,
                PoolId = new Random().Next(1000000).ToString(),
            };

            db.GetTable<FavoriteTasks>().InsertOnSubmit(favoriteTasks);
            db.SubmitChanges();

            await client.DeleteMessageAsync(GetChatId(update), messageId: msg.MessageId);
            await client.DeleteMessageAsync(GetChatId(update), messageId: msg.MessageId - 1);
            await client.DeleteMessageAsync(GetChatId(update), messageId: msg.MessageId - 2);
            FavoritetasksCommand favoriteTasksCommand = new FavoritetasksCommand();
            favoriteTasksCommand.Execute(update, client);
        }

        private async System.Threading.Tasks.Task ValidationOfYangToken(Update update, Message msg, DataContext db, Table<Users> users)
        {
            var obj = users.SingleOrDefault(row => row.Id == GetChatId(update));
            if (users.Any(c => c.Id == GetChatId(update)))
            {
                if (ParseYang.RequestToApiCheckToken(msg.Text).message == null)
                {
                    obj.Token = Encryption.EncryptStringToBytes(msg.Text, StaticFields.passwordEncryption);
                    db.SubmitChanges();
                    await client.SendTextMessageAsync(GetChatId(update),
                            "Токен записан",
                            replyToMessageId: msg.MessageId);
                }
                else
                {
                    await client.SendTextMessageAsync(GetChatId(update),
                            "Неверный токен" + StaticFields.LinkToManual,
                            replyToMessageId: msg.MessageId, parseMode: ParseMode.MarkdownV2);
                }
            }
        }

        public static long GetChatId(Update update)
        {
            if (update.Message != null)
                return update.Message.Chat.Id;
            else
                return update.CallbackQuery.Message.Chat.Id;
        }
    }
}