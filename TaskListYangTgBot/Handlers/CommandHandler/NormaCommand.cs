using System;
using System.Collections.Generic;
using System.Linq;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class NormaCommand : Command
    {
        public override string Keyword => "/norma";
        public async override void Execute(Update update, TelegramBotClient client)
        {
            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update), CreateMsgNorma(StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update))));
        }
        private string CreateMsgNorma(string tokenYang)
        {
            string completedEmoji = "🟩";
            string notCompletedEmoji = "⬜️";
            string progressBar;
            string message;
            List<NormValue> norm = ParseYang.RequestToApiCheckNormValue(tokenYang);
            if (norm != null)
            {
                int countCompletedEmoji = norm[1].currentNormValue < norm[1].destinationNormValue ? (int)norm[1].currentNormValue * 10 / (int)norm[1].destinationNormValue : 10;
                double countPercent = norm[1].destinationNormValue != 0 ? norm[1].currentNormValue * 100 / norm[1].destinationNormValue : 0;
                progressBar = string.Concat(Enumerable.Repeat(completedEmoji, countCompletedEmoji)) + string.Concat(Enumerable.Repeat(notCompletedEmoji, 10 - countCompletedEmoji));
                double remains = norm[1].currentNormValue < norm[1].destinationNormValue ? norm[1].destinationNormValue - norm[1].currentNormValue : 0;
                message = $"Выполнено - {norm[1].currentNormValue} БО\nЦель - {norm[1].destinationNormValue} БО\nОсталось - {Math.Round(remains, 3)} БО\n\n{Math.Round(countPercent, 3)}%\n{progressBar}";
            }
            else
            {
                message = "Нет нормы";
            }
            return message;
        }
    }
}
