using System;

namespace TaskListYangTgBot.Models
{
    class ActiveCount
    {
        public string request_id { get; set; }
        public string requestId { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public int count { get; set; }
        public DateTime next_expire_at { get; set; }
    }
}
