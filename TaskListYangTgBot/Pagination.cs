using System.Collections.Generic;
using System.Linq;

namespace TaskListYangTgBot
{
    internal class Pagination
    {
        public static Dictionary<long, int> numberOfPageDic = new Dictionary<long, int>();
        public void AddToDictionaryNumberPage(long chatId, int numberOfPage)
        {
            if (!numberOfPageDic.ContainsKey(chatId))
            {
                numberOfPageDic.Add(chatId, numberOfPage);
            }
            else
            {
                numberOfPageDic[chatId] = numberOfPage;
            }
        }
        public List<Root> GetPage(List<Root> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }
    }
}
