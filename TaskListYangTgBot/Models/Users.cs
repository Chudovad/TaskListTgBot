using System;
using System.Data.Linq.Mapping;

namespace TaskListYangTgBot
{
    [Table(Name = "Users")]
    public class Users
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public long Id { get; set; }
        [Column(Name = "UserName")]
        public string UserName { get; set; }
        [Column(Name = "FirstName")]
        public string FirstName { get; set; }
        [Column(Name = "LastName")]
        public string LastName { get; set; }
        [Column(Name = "DateReg")]
        public DateTime DateReg { get; set; }
        [Column(Name = "Payment", CanBeNull = false)]
        public bool Payment { get; set; }
        [Column(Name = "TypeSorting")]
        public int TypeSorting { get; set; }
        [Column(Name = "Token")]
        public byte[] Token { get; set; }
    }
}
