using EUC_form.Data;
using EUC_form.Models.Templates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EUC_form.DAL.Templates
{
    public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        #region Fields

        protected DbContext Context;

        #endregion

        public EfRepository(DbContext context)
        {
            Context = context;
        }

        #region Public Methods

        public ValueTask<T> GetById(int id)
        {
            return Context.Set<T>().FindAsync(id);
        }

        public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        // Add with save
        public async Task Add(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        // Add with save
        public Task Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return Context.SaveChangesAsync();
        }

        // Add with save
        public Task Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
            return Context.SaveChangesAsync();
        }
        public Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().AnyAsync<T>(predicate);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetOrderedBy(Expression<Func<T, string>> predicate, bool desc = false)
        {
            if (desc)
                return await Context.Set<T>().OrderByDescending(predicate).ToListAsync();
            else
                return await Context.Set<T>().OrderBy(predicate).ToListAsync();
        }

        public Task<int> CountAll()
        {
            return Context.Set<T>().CountAsync();
        }

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().CountAsync(predicate);
        }


        #endregion
    }
}
