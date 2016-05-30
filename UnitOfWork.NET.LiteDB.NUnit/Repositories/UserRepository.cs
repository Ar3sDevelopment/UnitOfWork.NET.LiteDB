using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.LiteDB.Classes;
using UnitOfWork.NET.LiteDB.NUnit.Data.Models;
using UnitOfWork.NET.LiteDB.NUnit.DTO;

namespace UnitOfWork.NET.LiteDB.NUnit.Repositories
{
    public class UserRepository : LiteRepository<User, UserDTO>
    {
        public UserRepository(IUnitOfWork manager) : base(manager)
        {
        }

        public IEnumerable<UserDTO> NewList()
        {
            Console.WriteLine("NewList");

            return List();
        }
    }
}
