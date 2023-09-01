using System;
using TaskListYangTgBot.Services;

namespace TaskListYangTgBot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TelegramBotHelper telegramBotHelper = new TelegramBotHelper();
                //DisplayConsole.hideWindow();
                telegramBotHelper.GetUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

