using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Caelan.DynamicLinq.Classes;
using Caelan.DynamicLinq.Extensions;
using ClassBuilder.Classes;
using ClassBuilder.Interfaces;
using LiteDB;
using UnitOfWork.NET.Classes;
using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.LiteDB.Interfaces;

namespace UnitOfWork.NET.LiteDB.Classes
{
    public class LiteRepository<TEntity> : Repository<TEntity>, ILiteRepository<TEntity> where TEntity : class, new()
    {
        public new ILiteUnitOfWork UnitOfWork => base.UnitOfWork as ILiteUnitOfWork;

        public LiteRepository(IUnitOfWork manager) : base(manager)
        {
        }

        public LiteCollection<TEntity> Collection => UnitOfWork.Collection<TEntity>();

        public IEnumerable<TEntity> All(Expression<Func<TEntity, bool>> expr) => Collection.Find(expr);

        public int Count(Expression<Func<TEntity, bool>> expr) => Collection.Count(expr);

        public TEntity Entity(Expression<Func<TEntity, bool>> expr) => Collection.FindOne(expr);

        public TEntity Entity(BsonValue id) => Collection.FindById(id);

        public bool Exists(Expression<Func<TEntity, bool>> expr) => Collection.Exists(expr);

        public virtual BsonValue Insert(TEntity entity) => Collection.Insert(entity);

        public virtual void Update(TEntity entity) => Collection.Update(entity);
        public virtual void Update(TEntity entity, BsonValue id) => Collection.Update(id, entity);

        public virtual void Delete(BsonValue id) => Collection.Delete(id);

        public async Task<BsonValue> InsertAsync(TEntity entity) => await new TaskFactory().StartNew(() => Insert(entity));
        public async Task UpdateAsync(TEntity entity) => await new TaskFactory().StartNew(() => Update(entity));
        public async Task UpdateAsync(TEntity entity, BsonValue id) => await new TaskFactory().StartNew(() => Update(entity, id));
        public async Task DeleteAsync(BsonValue id) => await new TaskFactory().StartNew(() => Delete(id));
        public async Task<TEntity> EntityAsync(BsonValue id) => await new TaskFactory().StartNew(() => Entity(id));
        public async Task<TEntity> EntityAsync(Expression<Func<TEntity, bool>> expr) => await new TaskFactory().StartNew(() => Entity(expr));
    }

    public class LiteRepository<TEntity, TDTO> : LiteRepository<TEntity>, ILiteRepository<TEntity, TDTO> where TEntity : class, new() where TDTO : class, new()
    {
        public IMapper<TEntity, TDTO> DTOMapper { get; set; }
        public IMapper<TDTO, TEntity> EntityMapper { get; set; }

        public LiteRepository(IUnitOfWork manager) : base(manager)
        {
        }

        public TDTO ElementBuilt(Func<TEntity, bool> expr) => DTO(Expression.Lambda<Func<TEntity, bool>>(Expression.Call(expr.Method)));

        public IEnumerable<TDTO> AllBuilt() => List();

        public IEnumerable<TDTO> AllBuilt(Func<TEntity, bool> expr) => List(Expression.Lambda<Func<TEntity, bool>>(Expression.Call(expr.Method)));

        public DataSourceResult<TDTO> DataSource(int take, int skip, ICollection<Sort> sort, Filter filter, Func<TEntity, bool> expr) => DataSource(take, skip, sort, filter, Expression.Lambda<Func<TEntity, bool>>(Expression.Call(expr.Method)));

        public TDTO DTO(BsonValue id)
        {
            var entity = Entity(id);

            return entity != null ? Builder.Build(entity).To<TDTO>() : null;
        }

        public TDTO DTO(Expression<Func<TEntity, bool>> expr)
        {
            var entity = Entity(expr);

            return entity != null ? Builder.Build(entity).To<TDTO>() : null;
        }

        public IEnumerable<TDTO> List() => Builder.BuildList(All()).ToList<TDTO>();

        public IEnumerable<TDTO> List(Expression<Func<TEntity, bool>> expr) => Builder.BuildList(All(expr)).ToList<TDTO>();

        public DataSourceResult<TDTO> DataSource(int take, int skip, ICollection<Sort> sort, Filter filter, Expression<Func<TEntity, bool>> expr) => DataSource(take, skip, sort, filter, expr, t => Builder.BuildList(t).ToList<TDTO>());

        private DataSourceResult<TDTO> DataSource(int take, int skip, ICollection<Sort> sort, Filter filter, Expression<Func<TEntity, bool>> expr, Func<IEnumerable<TEntity>, IEnumerable<TDTO>> buildFunc)
        {
            var orderBy = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(t => t.Name).FirstOrDefault();

            var res = All(expr);

            if (orderBy != null)
                res = res.OrderBy(orderBy);

            var ds = res.AsQueryable().ToDataSourceResult(take, skip, sort, filter);


            return new DataSourceResult<TDTO>
            {
                Data = buildFunc(ds.Data).ToList(),
                Total = ds.Total
            };
        }

        public BsonValue Insert(TDTO dto) => Insert(Builder.Build(dto).To<TEntity>());

        public void Update(TDTO dto, BsonValue id)
        {
            var entity = Entity(id);
            Builder.Build(dto).To(entity);
            Update(entity, id);
        }

        public void Update(TDTO dto)
        {
            Update(Builder.Build(dto).To<TEntity>());
        }

        public async Task<BsonValue> InsertAsync(TDTO dto) => await new TaskFactory().StartNew(() => Insert(dto));
        public async Task UpdateAsync(TDTO dto) => await new TaskFactory().StartNew(() => Update(dto));
        public async Task UpdateAsync(TDTO dto, BsonValue id) => await new TaskFactory().StartNew(() => Update(dto, id));
        public async Task<IEnumerable<TDTO>> ListAsync(Expression<Func<TEntity, bool>> expr) => await new TaskFactory().StartNew(() => List(expr));
        public async Task<IEnumerable<TDTO>> ListAsync() => await new TaskFactory().StartNew(List);
        public async Task<DataSourceResult<TDTO>> DataSourceAsync(int take, int skip, ICollection<Sort> sort, Filter filter, Expression<Func<TEntity, bool>> expr) => await new TaskFactory().StartNew(() => DataSource(take, skip, sort, filter, expr));
        public async Task<TDTO> DTOAsync(Expression<Func<TEntity, bool>> expr) => await new TaskFactory().StartNew(() => DTO(expr));
        public async Task<TDTO> DTOAsync(BsonValue id) => await new TaskFactory().StartNew(() => DTO(id));
    }
}
