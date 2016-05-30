using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Caelan.DynamicLinq.Classes;
using LiteDB;
using UnitOfWork.NET.Interfaces;

namespace UnitOfWork.NET.LiteDB.Interfaces
{
    public interface ILiteRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        new ILiteUnitOfWork UnitOfWork { get; }

        LiteCollection<TEntity> Collection { get; }

        TEntity Entity(BsonValue id);

        TEntity Entity(Expression<Func<TEntity, bool>> expr);

        IEnumerable<TEntity> All(Expression<Func<TEntity, bool>> expr);

        bool Exists(Expression<Func<TEntity, bool>> expr);

        int Count(Expression<Func<TEntity, bool>> expr);

        BsonValue Insert(TEntity entity);

        void Update(TEntity entity);
        void Update(TEntity entity, BsonValue id);

        void Delete(BsonValue id);
    }

    public interface ILiteRepository<TEntity, TDTO> : IRepository<TEntity, TDTO>, ILiteRepository<TEntity> where TEntity : class, new() where TDTO : class, new()
    {
        TDTO DTO(BsonValue id);

        TDTO DTO(Expression<Func<TEntity, bool>> expr);

        IEnumerable<TDTO> List();

        IEnumerable<TDTO> List(Expression<Func<TEntity, bool>> expr);

        DataSourceResult<TDTO> DataSource(int take, int skip, ICollection<Sort> sort, Filter filter, Expression<Func<TEntity, bool>> where);

        BsonValue Insert(TDTO dto);

        void Update(TDTO dto, BsonValue id);
        void Update(TDTO dto);
    }
}
