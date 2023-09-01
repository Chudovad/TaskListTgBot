using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class YangOnFavoriteCommand : Command
    {
        public override string Keyword => "/yangonfavorite";

        public async override void Execute(Update update, TelegramBotClient client)
        {
            YangOnCommand yangOnCommand = new YangOnCommand();
            if (CommandStatus.commandStatus[update.Message.Chat.Id] == false)
            {
                yangOnCommand.StartYangONCommand(update, true, client);
            }
            else
            {
                await client.SendTextMessageAsync(update.Message.Chat.Id, "Команда /yangon или /yangonfavorite уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
