using System;
using System.Data.Linq;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class StartCommand : Command
    {
        public override string Keyword => "/start";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            Table<Users> users = db.GetTable<Users>();
            if (!users.Any(c => c.Id == TelegramBotHelper.GetChatId(update)))
            {
                Users newUser = new Users
                {
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName,
                    Id = TelegramBotHelper.GetChatId(update),
                    UserName = update.Message.Chat.Username,
                    DateReg = DateTime.Now.AddHours(StaticFields.countAddHours),
                    Payment = false,
                    Token = Encryption.EncryptStringToBytes("", StaticFields.passwordEncryption),
                    TypeSorting = 2
                };

                db.GetTable<Users>().InsertOnSubmit(newUser);
                db.SubmitChanges();
            }

            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), "Привет\\!" + StaticFields.HelpMsg, parseMode: ParseMode.MarkdownV2);
            db.Dispose();
        }
    }
}
