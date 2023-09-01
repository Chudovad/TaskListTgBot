using System;
using System.Data.Linq.Mapping;

namespace TaskListYangTgBot
{
    [Table(Name = "Messages")]
    class Messages
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(Name = "Msg")]
        public string Msg { get; set; }
        [Column(Name = "UserId")]
        public long UserId { get; set; }
        [Column(Name = "DateTime")]
        public DateTime DateTime { get; set; }
    }
}
