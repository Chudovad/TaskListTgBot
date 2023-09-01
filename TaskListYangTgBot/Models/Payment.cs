using System.Data.Linq.Mapping;

namespace TaskListYangTgBot
{
    [Table(Name = "Payment")]
    class Payment
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true)]
        public long Id { get; set; }
        [Column(Name = "UserId")]
        public int UserId { get; set; }
        [Column(Name = "UserInfo")]
        public string UserInfo { get; set; }
        [Column(Name = "PaymentId")]
        public string PaymentId { get; set; }

    }
}
