using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiteDB;
using UnitOfWork.NET.Interfaces;

namespace UnitOfWork.NET.LiteDB.Interfaces
{
    public interface ILiteUnitOfWork : IUnitOfWork
    {
        LiteCollection<TEntity> Collection<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        void Transaction(Action<ILiteUnitOfWork> body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        bool TransactionSaveChanges(Action<ILiteUnitOfWork> body);

        /// <summary>
        /// 
        /// </summary>
        new ILiteRepository<TEntity> Repository<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 
        /// </summary>
        new ILiteRepository<TEntity, TDTO> Repository<TEntity, TDTO>() where TEntity : class, new() where TDTO : class, new();
    }
}
