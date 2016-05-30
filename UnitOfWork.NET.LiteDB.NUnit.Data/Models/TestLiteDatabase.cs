using LiteDB;

namespace UnitOfWork.NET.LiteDB.NUnit.Data.Models
{
    public class TestLiteDatabase : LiteDatabase
    {
        public TestLiteDatabase() : base("test.db")
        {
            Roles = GetCollection<Role>(typeof(Role).Name);
            Users = GetCollection<User>(typeof(User).Name);
        }

        public LiteCollection<Role> Roles { get; set; }
        public LiteCollection<User> Users { get; set; }
    }
}
