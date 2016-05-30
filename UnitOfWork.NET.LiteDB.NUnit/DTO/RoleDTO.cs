using System.Collections.Generic;

namespace UnitOfWork.NET.LiteDB.NUnit.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public virtual ICollection<UserDTO> Users { get; set; }
    }
}
