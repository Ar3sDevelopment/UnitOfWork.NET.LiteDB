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
        private readonly LiteDatabase _dbContext;

        public LiteUnitOfWork(LiteDatabase context) : this(context, false)
        {
        }

        internal LiteUnitOfWork(LiteDatabase context, bool managedContext)
        {
            _dbContext = context;
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
                _dbContext.Dispose();

            base.Dispose();
        }

        public virtual void BeforeSaveChanges(LiteDatabase context)
        {
        }

        public void SaveChanges()
        {
            //_dbContext.ChangeTracker.DetectChanges();

            //var entries = _dbContext.ChangeTracker.Entries();
            //var entriesGroup = entries.Where(t => t.State != EntityState.Unchanged && t.State != EntityState.Detached).ToList().GroupBy(t => ObjectContext.GetObjectType(t.Entity.GetType())).ToList().Select(t => new { t.Key, EntriesByState = t.GroupBy(g => g.State, g => g.Entity).ToList() }).ToList();

            //BeforeSaveChanges(_dbContext);

            //var res = _dbContext.SaveChanges();

            //foreach (var item in entriesGroup)
            //{
            //	var entityType = item.Key;
            //	var entitiesByState = item.EntriesByState.ToDictionary(t => t.Key, t => t.AsEnumerable());
            //	var mHelper = typeof(EntityUnitOfWork).GetMethod("CallOnSaveChanges", BindingFlags.NonPublic | BindingFlags.Instance);
            //	mHelper.MakeGenericMethod(entityType).Invoke(this, new object[] { entitiesByState });
            //}

            //AfterSaveChanges(_dbContext);


            _dbContext.Commit();
        }

        public virtual void AfterSaveChanges(LiteDatabase context)
        {
        }

        public override IEnumerable<TEntity> Data<TEntity>() => Collection<TEntity>().FindAll();

        public void Transaction(Action<ILiteUnitOfWork> body)
        {
            _dbContext.BeginTrans();

            try
            {
                body.Invoke(this);
                _dbContext.Commit();
            }
            catch
            {
                _dbContext.Rollback();
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
            _dbContext.BeginTrans();
            try
            {
                body.Invoke(this);
                SaveChanges();
                _dbContext.Commit();
                return true;
            }
            catch
            {
                _dbContext.Rollback();
                return false;
            }
        }

        //public DbEntityEntry Entry<TEntity>(TEntity entity) where TEntity : class => _dbContext.Entry(entity);

        public LiteCollection<TEntity> Collection<TEntity>() where TEntity : class, new() => _dbContext.GetCollection<TEntity>(typeof(TEntity).Name);

        public new ILiteRepository<TEntity> Repository<TEntity>() where TEntity : class, new() => base.Repository<TEntity>() as ILiteRepository<TEntity>;
        public new ILiteRepository<TEntity, TDTO> Repository<TEntity, TDTO>() where TEntity : class, new() where TDTO : class, new() => base.Repository<TEntity, TDTO>() as ILiteRepository<TEntity, TDTO>;

        //private void CallOnSaveChanges<TEntity>(Dictionary<EntityState, IEnumerable<object>> entitiesObj) where TEntity : class
        //{
        //    var entities = entitiesObj.ToDictionary(t => t.Key, t => t.Value.Cast<TEntity>());

        //    Repository<TEntity>().OnSaveChanges(entities);
        //}

        public async Task SaveChangesAsync() => await new TaskFactory().StartNew(SaveChanges);
    }

    public class LiteUnitOfWork<TContext> : LiteUnitOfWork where TContext : LiteDatabase, new()
    {
        public LiteUnitOfWork() : base(new TContext(), true)
        {
        }

        public override void BeforeSaveChanges(LiteDatabase context)
        {
            BeforeSaveChanges(context as TContext);
        }

        public override void AfterSaveChanges(LiteDatabase context)
        {
            AfterSaveChanges(context as TContext);
        }

        public virtual void BeforeSaveChanges(TContext context)
        {
            base.BeforeSaveChanges(context);
        }

        public virtual void AfterSaveChanges(TContext context)
        {
            base.AfterSaveChanges(context);
        }
    }
}
