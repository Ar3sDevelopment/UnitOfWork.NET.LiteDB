using ClassBuilder.Classes;
using UnitOfWork.NET.LiteDB.NUnit.Data.Models;
using UnitOfWork.NET.LiteDB.NUnit.DTO;

namespace UnitOfWork.NET.LiteDB.NUnit.Mappers
{
    public class UserMapper : DefaultMapper<UserDTO, User>
    {
        public override User CustomMap(UserDTO source, User destination)
        {
            var res = base.CustomMap(source, destination);

            res.Id = source.Id;
            res.Login = source.Login;
            res.Password = source.Password;

            return res;
        }
    }
}
