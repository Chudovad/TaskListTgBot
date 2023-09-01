using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaskListYangTgBot.Services;

namespace TaskListYangTgBot
{
    internal class CommandStatus
    {
        public static Dictionary<long, List<Root>> taskListsByChatId = new Dictionary<long, List<Root>>();
        public static Dictionary<long, bool> commandStatus = new Dictionary<long, bool>();

        private List<Root> GetFilteredTasks(string tokenYang)
        {
            return ParseYang.RequestToApiTaskList(tokenYang)
                            .Where(x => x.projectMetaInfo.ignored != true)
                            .ToList();
        }

        public void AddToDictionaryTaskList(int sortingType, string tokenYang, long chatId)
        {
            List<Root> filteredTasks = GetFilteredTasks(tokenYang);

            if (!taskListsByChatId.ContainsKey(chatId))
            {
                taskListsByChatId.Add(chatId, filteredTasks);
            }
            else
            {
                List<Root> sortedTasks;

                if (sortingType == 1)
                {
                    sortedTasks = filteredTasks.OrderBy(r => double.Parse(r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
                }
                else if (sortingType == 0)
                {
                    sortedTasks = filteredTasks.OrderByDescending(r => double.Parse(r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
                }
                else
                {
                    sortedTasks = filteredTasks;
                }

                taskListsByChatId[chatId] = sortedTasks;
            }
        }

    }
}
