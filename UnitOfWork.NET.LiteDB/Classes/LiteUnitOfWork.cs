using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Autofac;
using LiteDB;
using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.LiteDB.Extenders;
using UnitOfWork.NET.LiteDB.Interfaces;

namespace UnitOfWork.NET.LiteDB.Classes
{
    public class LiteUnitOfWork : NET.Classes.UnitOfWork, ILiteUnitOfWork
    {
        private readonly bool _autoContext;
        private readonly LiteDatabase _database;

        public LiteUnitOfWork(LiteDatabase context) : this(context, false)
        {
        }

        internal LiteUnitOfWork(LiteDatabase context, bool managedContext)
        {
            _database = context;
            _autoContext = managedContext;

            var cb = new ContainerBuilder();

            cb.RegisterGeneric(typeof(LiteRepository<>)).AsSelf().As(typeof(ILiteRepository<>)).As(typeof(IRepository<>));
            cb.RegisterGeneric(typeof(LiteRepository<,>)).AsSelf().As(typeof(ILiteRepository<,>)).As(typeof(IRepository<,>));

            UpdateContainer(cb);
            UpdateProperties();
        }

        public override void Dispose()
        {
            if (_autoContext)
                _database.Dispose();

            base.Dispose();
        }

        public override IEnumerable<TEntity> Data<TEntity>() => Collection<TEntity>().FindAll();

        public void Transaction(Action<ILiteUnitOfWork> body)
        {
            _database.BeginTrans();

            try
            {
                body.Invoke(this);
                _database.Commit();
            }
            catch
            {
                _database.Rollback();
            }
        }

        protected override void RegisterRepository(ContainerBuilder cb, Type repositoryType)
        {
            base.RegisterRepository(cb, repositoryType);

            if (repositoryType.IsGenericTypeDefinition)
                cb.RegisterGeneric(repositoryType).AsSelf().AsLiteRepository().AsImplementedInterfaces();
            else
                cb.RegisterType(repositoryType).AsSelf().AsLiteRepository().AsImplementedInterfaces();
        }

        public bool TransactionSaveChanges(Action<ILiteUnitOfWork> body)
        {
            _database.BeginTrans();
            try
            {
                body.Invoke(this);
                _database.Commit();
                return true;
            }
            catch
            {
                _database.Rollback();
                return false;
            }
        }

        public LiteCollection<TEntity> Collection<TEntity>() where TEntity : class, new() => _database.GetCollection<TEntity>(typeof(TEntity).Name);

        public new ILiteRepository<TEntity> Repository<TEntity>() where TEntity : class, new() => base.Repository<TEntity>() as ILiteRepository<TEntity>;
        public new ILiteRepository<TEntity, TDTO> Repository<TEntity, TDTO>() where TEntity : class, new() where TDTO : class, new() => base.Repository<TEntity, TDTO>() as ILiteRepository<TEntity, TDTO>;

    }

    public class LiteUnitOfWork<TContext> : LiteUnitOfWork where TContext : LiteDatabase, new()
    {
        public LiteUnitOfWork() : base(new TContext(), true)
        {
        }
    }
}
