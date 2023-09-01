using System;
using System.Data.Linq;

namespace TaskListYangTgBot
{
    internal class WriteToDB
    {
        public static void WriteMsgToDB(string message, long chatId)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            Messages msg = new Messages
            {
                Msg = message,
                UserId = chatId,
                DateTime = DateTime.Now.AddHours(StaticFields.countAddHours)
            };
            db.GetTable<Messages>().InsertOnSubmit(msg);
            db.SubmitChanges();
            db.Dispose();
        }
        public static void WriteLogToDB(long userId, string message, Exception ex)
        {
            DataContext db = new DataContext(StaticFields.connectionString);
            Logs log = new Logs
            {
                Message = message,
                Exception = ex.ToString(),
                UserId = userId,
                DateTime = DateTime.Now.AddHours(StaticFields.countAddHours)
            };
            db.GetTable<Logs>().InsertOnSubmit(log);
            db.SubmitChanges();
            db.Dispose();
        }
    }
}
