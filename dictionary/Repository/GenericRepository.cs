using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using dictionary.Model;

namespace dictionary.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        public IDbTransaction _transaction { get; set; }
        public GenericRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(T model)
        {
            var type = model.GetType();
            var obj = type.GetProperties();

            var sql = "insert into user (Username,PasswordHash,PasswordSalt) values (@name,@hash,@salt)";
            await connection.OpenAsync();
            var data = await connection.ExecuteAsync(sql, new { name = obj.GetValue(1), hash = obj.GetValue(2), salt = obj.GetValue(3) });
            if (data != 0)
            {
                return await Task.FromResult(true);
            }
            connection.Close();

            return await Task.FromResult(false);
        }

        public Task<T> Update(T model)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Close();
                    connection.Dispose();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GenericRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
