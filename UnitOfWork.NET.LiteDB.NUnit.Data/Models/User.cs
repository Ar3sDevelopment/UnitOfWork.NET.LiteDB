using System.Collections.Generic;
using LiteDB;

namespace UnitOfWork.NET.LiteDB.NUnit.Data.Models
{
    public class User
    {
        [BsonId]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; }
    }
}
