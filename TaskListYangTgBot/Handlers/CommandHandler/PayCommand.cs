using System;
using System.Collections.Generic;
using TaskListYangTgBot.Services;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace TaskListYangTgBot.Handlers.CommandHandler
{
    internal class PayCommand : Command
    {
        public override string Keyword => "/pay";
        public async override void Execute(Update update, TelegramBotClient client)
        {
            List<LabeledPrice> myList = new List<LabeledPrice>
                    {
                        new LabeledPrice("Цена", Convert.ToInt32("10000"))
                    };
            //await client.SendInvoiceAsync(TelegramBotHelper.GetChatId(update), "Оплата бота", "Это разовый платёж для безграничного доступа к боту", "payload", StaticFields.tokenPayment, "RUB", myList);

        }
    }
}
