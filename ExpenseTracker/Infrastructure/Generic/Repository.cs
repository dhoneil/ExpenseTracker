using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Generic
{
    public abstract class Repository<TEntity> where TEntity : class
    {
        protected _ApplicationDbContext dbContext;
        protected Repository()
        {
            if (dbContext == null)
            {
                dbContext = new _ApplicationDbContext();
            }
        }

        public virtual void Create(TEntity obj)
        {
            dbContext.Entry(obj).State = EntityState.Added;
            dbContext.SaveChanges();
        }

        public virtual TEntity AddNew(TEntity obj)
        {
            var res = dbContext.Entry(obj).State = EntityState.Added;
            dbContext.SaveChangesAsync();
            return obj;
        }
        //Add List of new Entities
        public virtual void Create(List<TEntity> objs)
        {
            objs.ForEach(item => dbContext.Entry(item).State = EntityState.Added);
            dbContext.SaveChanges();
        }
        //Update Entity
        public virtual void Update(TEntity obj)
        {
            dbContext.Entry(obj).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        //Update List of Entitities
        public virtual void Update(List<TEntity> objs)
        {
            objs.ForEach(item => dbContext.Entry(item).State = EntityState.Modified);
            dbContext.SaveChanges();
        }

        //Delete Entity
        public virtual void Delete(int id)
        {
            var dbObj = dbContext.Set<TEntity>().Find(id);
            dbContext.Entry(dbObj).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }

        //Delete Entity
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var dbSet = dbContext.Set<TEntity>();
            dbSet.RemoveRange(dbSet.Where(predicate));
            dbContext.SaveChanges();
        }

        //Get Entity by Id
        public virtual TEntity Get(int id)
        {
            return dbContext.Set<TEntity>().Find(id);
        }

        //Get All Entities
        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbContext.Set<TEntity>().AsNoTracking().ToList();
        }
        public virtual IEnumerable<TEntity> GetAll(int pagesize, int pageindex)
        {
            return dbContext.Set<TEntity>().Skip(pageindex - 1 * pagesize).Take(pagesize).ToList();
        }

        //Get All Entities
        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Set<TEntity>().AsNoTracking().Where(predicate);
        }

        public virtual void Detach(TEntity entity)
        {
            dbContext.Entry(entity).State = EntityState.Detached;
        }

        public IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
    }
}