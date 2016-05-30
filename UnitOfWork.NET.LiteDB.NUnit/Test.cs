using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnitOfWork.NET.LiteDB.Classes;
using UnitOfWork.NET.LiteDB.NUnit.Classes;
using UnitOfWork.NET.LiteDB.NUnit.Data.Models;
using UnitOfWork.NET.LiteDB.NUnit.DTO;
using UnitOfWork.NET.LiteDB.NUnit.Repositories;

namespace UnitOfWork.NET.LiteDB.NUnit
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestLiteDatabase()
        {
            using (var db = new TestLiteDatabase())
            {
                var adminRole = new Role { Description = "Admin" };
                var userRole = new Role { Description = "User" };
                db.Roles.Insert(adminRole);
                db.Roles.Insert(userRole);

                db.Users.Insert(new User
                {
                    Login = "test",
                    Password = "test",
                    Roles = new List<Role> { adminRole }
                });

                foreach (var role in db.Roles.FindAll())
                {
                    Console.WriteLine($"[{role.Id}] {role.Description}");
                }

                foreach (var user in db.Users.FindAll())
                {
                    Console.WriteLine($"[{user.Id}] {user.Login}:{user.Password}");
                }

                db.Users.Delete(t => t.Login != null);
                db.Roles.Delete(t => t.Description != null);
            }
        }

        [Test]
        public void TestEntityRepository()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new LiteUnitOfWork<TestLiteDatabase>())
            {
                var users = uow.Repository<User>().All();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");

                var entity = new User
                {
                    Login = "test",
                    Password = "test"
                };

                uow.Repository<User>().Insert(entity);
                Assert.AreNotEqual(entity.Id, 0);
                Console.WriteLine(entity.Id);
                entity.Password = "test2";
                uow.Repository<User>().Update(entity, entity.Id);
                Assert.AreEqual(entity.Password, uow.Repository<User>().Entity(entity.Id).Password);
                uow.Repository<User>().Delete(entity.Id);
                Assert.IsNull(uow.Repository<User>().Entity(entity.Id));
            }
        }

        [Test]
        public void TestDTORepository()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new LiteUnitOfWork<TestLiteDatabase>())
            {
                var users = uow.Repository<User, UserDTO>().List();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");

                var dto = new UserDTO
                {
                    Login = "test",
                    Password = "test"
                };

                uow.Repository<User, UserDTO>().Insert(dto);
                var login = dto.Login;
                dto = uow.Repository<User, UserDTO>().DTO(d => d.Login == login);
                Assert.IsNotNull(dto);
                Assert.AreNotEqual(dto.Id, 0);
                Console.WriteLine(dto.Id);
                dto.Password = "test2";
                uow.Repository<User, UserDTO>().Update(dto, dto.Id);
                Assert.AreEqual(dto.Password, uow.Repository<User, UserDTO>().DTO(dto.Id).Password);
                uow.Repository<User, UserDTO>().Delete(dto.Id);
                Assert.IsNull(uow.Repository<User, UserDTO>().DTO(dto.Id));
            }
        }

        [Test]
        public void TestCustomRepository()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new LiteUnitOfWork<TestLiteDatabase>())
            {
                var users = uow.CustomRepository<UserRepository>().NewList();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");
                var dto = new UserDTO
                {
                    Login = "test",
                    Password = "test"
                };

                uow.CustomRepository<UserRepository>().Insert(dto);
                var login = dto.Login;
                dto = uow.CustomRepository<UserRepository>().DTO(d => d.Login == login);
                Assert.IsNotNull(dto);
                Assert.AreNotEqual(dto.Id, 0);
                Console.WriteLine(dto.Id);
                dto.Password = "test2";
                uow.CustomRepository<UserRepository>().Update(dto, dto.Id);
                Assert.AreEqual(dto.Password, uow.CustomRepository<UserRepository>().DTO(dto.Id).Password);
                uow.CustomRepository<UserRepository>().Delete(dto.Id);
                Assert.IsNull(uow.CustomRepository<UserRepository>().DTO(dto.Id));
            }
        }

        [Test]
        public void TestCustomUnitOfWork()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new TestUnitOfWork())
            {
                var users = uow.Users.NewList();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");
                var dto = new UserDTO
                {
                    Login = "test",
                    Password = "test"
                };
                uow.Users.Insert(dto);
                var login = dto.Login;
                dto = uow.Users.DTO(d => d.Login == login);
                Assert.IsNotNull(dto);
                Assert.AreNotEqual(dto.Id, 0);
                Console.WriteLine(dto.Id);
                dto.Password = "test2";
                uow.Users.Update(dto, dto.Id);
                Assert.AreEqual(dto.Password, uow.Users.DTO(dto.Id).Password);
                uow.Users.Delete(dto.Id);
                Assert.IsNull(uow.Users.DTO(dto.Id));
            }
        }

        [Test]
        public void TestReflection()
        {
            using (var uow = new TestUnitOfWork())
            {
                Assert.IsNotNull(uow.Repository<User, UserDTO>(), "uow.Users != null by reflection");
                Assert.IsNotNull(uow.Repository<Role, RoleDTO>(), "uow.Roles != null by reflection");

                Assert.IsNotNull(uow.Users, "uow.Users != null");
                Assert.IsNotNull(uow.Roles, "uow.Roles != null");
            }
        }
    }
}

