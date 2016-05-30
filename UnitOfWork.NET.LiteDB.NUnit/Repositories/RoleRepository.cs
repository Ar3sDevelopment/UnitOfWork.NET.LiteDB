using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.LiteDB.Classes;
using UnitOfWork.NET.LiteDB.NUnit.Data.Models;
using UnitOfWork.NET.LiteDB.NUnit.DTO;

namespace UnitOfWork.NET.LiteDB.NUnit.Repositories
{
    public class RoleRepository : LiteRepository<Role, RoleDTO>
    {
        public RoleRepository(IUnitOfWork manager) : base(manager)
        {
        }
    }
}
