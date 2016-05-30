using System.Collections.Generic;
using LiteDB;

namespace UnitOfWork.NET.LiteDB.NUnit.Data.Models
{
    public class Role
    {
        [BsonId]
        public int Id { get; set; }
        public string Description { get; set; }
        public List<User> Users { get; set; }
    }
}
