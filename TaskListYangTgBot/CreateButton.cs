using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangTgBot
{
    internal class CreateButton
    {
        public static IReplyMarkup GetButton(List<FavoriteTasks> favoriteTasks)
        {
            List<InlineKeyboardButton[]> inlineKeyboard = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < favoriteTasks.Count; i++)
            {
                inlineKeyboard.Add(new InlineKeyboardButton[] { new InlineKeyboardButton(favoriteTasks[i].FavoriteTask) });
                inlineKeyboard[i][0].CallbackData = "FavoriteTask" + favoriteTasks[i].PoolId;
            }

            // Keyboard
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(inlineKeyboard.ToArray());

            return keyboard;
        }
        public static IReplyMarkup GetButton(List<string> textButtons)
        {
            List<InlineKeyboardButton[]> inlineKeyboard = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < textButtons.Count; i++)
            {
                inlineKeyboard.Add(new InlineKeyboardButton[] { new InlineKeyboardButton(textButtons[i]) });
                inlineKeyboard[i][0].CallbackData = textButtons[i];
            }

            // Keyboard
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(inlineKeyboard.ToArray());

            return keyboard;
        }

        public static IReplyMarkup GetButton(int id, string textButton)
        {
            InlineKeyboardButton keyboardButton = new InlineKeyboardButton(textButton)
            {
                CallbackData = id.ToString() + textButton
            };
            return new InlineKeyboardMarkup(keyboardButton);
        }

        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2 } }
            };
            return new InlineKeyboardMarkup(keyboardButton);
        }
        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2, string url)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2, Url = url } }
            };
            return new InlineKeyboardMarkup(keyboardButton);
        }
        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2, string textButton3, string urlFor2, string urlFor3)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2, Url = urlFor2 } },
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton3) { CallbackData = id.ToString() + textButton3, Url = urlFor3 } }

            };
            return new InlineKeyboardMarkup(keyboardButton);
        }
        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2, string textButton3, string textButton4, string urlFor2, string urlFor3, string urlTestStand)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2, Url = urlFor2 } },
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton3) { CallbackData = id.ToString() + textButton3, Url = urlFor3 } },
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton4) { CallbackData = id.ToString() + textButton4, Url = urlTestStand } }

            };
            return new InlineKeyboardMarkup(keyboardButton);
        }
        public static IReplyMarkup GetButton(RootTakeTaskResponse takeTaskResponse, string linkTask, string checkEnvironment, string urlTestStand)
        {
            IReplyMarkup replyMarkup;
            if (checkEnvironment == "" && !Uri.IsWellFormedUriString(urlTestStand, UriKind.Absolute))
            {
                replyMarkup = GetButton(takeTaskResponse.poolId, "Выйти", "Ссылка на задание", linkTask);
            }
            else if (Uri.IsWellFormedUriString(urlTestStand, UriKind.Absolute) && checkEnvironment == "")
            {
                replyMarkup = GetButton(takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на тестовый стенд", linkTask, urlTestStand);
            }
            else if (!Uri.IsWellFormedUriString(urlTestStand, UriKind.Absolute) && checkEnvironment != "")
            {
                replyMarkup = GetButton(takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на проверку окружения", linkTask, checkEnvironment);
            }
            else
            {
                replyMarkup = GetButton(takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на проверку окружения", "Ссылка на тестовый стенд", linkTask, checkEnvironment, urlTestStand);
            }

            return replyMarkup;

        }
    }
}
