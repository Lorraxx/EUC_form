using EUC_form.Models.Templates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EUC_form.DAL.Templates
{
    public class JSONRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly string _filePath;

        protected JsonSerializerOptions serializerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };

        public JSONRepository()
        {
            // Todo: Put file path in config instead
            this._filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Database.json";

            if (!File.Exists(this._filePath))
                File.Create(this._filePath);
        }
        public async Task Add(T entity)
        {
            // Todo: resolve concurent access to db file

            List<T> listT = (await this.GetAll()).Append(entity).ToList();

            string JSON_string = JsonSerializer.Serialize(listT,
                                                   listT.GetType(),
                                                   serializerOptions);
            try
            {
                using (StreamWriter writer = new StreamWriter(this._filePath, append: false))
                {
                    await writer.WriteAsync(JSON_string);
                }
            }
            catch (Exception)
            {
                // Todo: Handle file exceptions
                throw;
            }
        }

        public Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAll()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            if (!File.Exists(this._filePath))
                throw new FileNotFoundException("File was not found!", this._filePath);
            try
            {
                using (StreamReader reader = new StreamReader(this._filePath))
                {
                    string json = await reader.ReadToEndAsync();
                    List<T> items = new List<T>();
                    try
                    {
                        items = JsonSerializer.Deserialize(json, items.GetType(), serializerOptions) as List<T>;
                    }
                    catch (Exception)
                    {
                        // Todo: Handle JSON deserialization exceptions
                    }
                    return items;
                }
            }
            catch (Exception)
            {
                // Todo: Handle file exceptions
                throw;
            }

        }

        public ValueTask<T> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetOrderedBy(Expression<Func<T, string>> predicate, bool desc = false)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
