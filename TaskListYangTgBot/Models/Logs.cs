using System;
using System.Data.Linq.Mapping;

namespace TaskListYangTgBot
{
    [Table(Name = "Logs")]
    class Logs
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(Name = "Message")]
        public string Message { get; set; }
        [Column(Name = "Exception")]
        public string Exception { get; set; }
        [Column(Name = "UserId")]
        public long UserId { get; set; }
        [Column(Name = "DateTime")]
        public DateTime DateTime { get; set; }
    }
}
