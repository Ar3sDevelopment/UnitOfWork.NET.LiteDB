using System.Collections.Generic;

namespace UnitOfWork.NET.LiteDB.NUnit.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public IEnumerable<RoleDTO> Roles { get; set; }
    }
}
