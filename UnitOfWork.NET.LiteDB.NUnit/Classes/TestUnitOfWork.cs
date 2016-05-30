using UnitOfWork.NET.LiteDB.Classes;
using UnitOfWork.NET.LiteDB.NUnit.Data.Models;
using UnitOfWork.NET.LiteDB.NUnit.Repositories;

namespace UnitOfWork.NET.LiteDB.NUnit.Classes
{
	public class TestUnitOfWork : LiteUnitOfWork<TestLiteDatabase>
	{
		public UserRepository Users { get; set; }
		public RoleRepository Roles { get; set; }
	}
}
