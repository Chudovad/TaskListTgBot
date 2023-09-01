using System.Data.Linq.Mapping;

namespace TaskListYangTgBot
{
    [Table(Name = "FavoriteTasks")]
    class FavoriteTasks
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true)]
        public long Id { get; set; }
        [Column(Name = "UserId")]
        public string UserId { get; set; }
        [Column(Name = "FavoriteTask")]
        public string FavoriteTask { get; set; }

        [Column(Name = "PoolId")]
        public string PoolId { get; set; }
    }
}
