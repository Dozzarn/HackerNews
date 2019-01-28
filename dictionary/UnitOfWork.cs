using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using dictionary.Helpers;
using dictionary.Repository;
using Microsoft.Extensions.Configuration;

namespace dictionary
{
    public class UnitOfWork : IUnitOfWork
    {
        //TODO:Logger


        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public IAuthRepository _authRepository { get; set; }
        public ITitleRepository _titleRepository { get; set; }
        public RedisHandler _redisHandler { get; set; }

        public IConfiguration _configuration { get; set; }
        private bool disposedValue = false; // To detect redundant calls


        public UnitOfWork(IConfiguration configuration)
        {
            _redisHandler = new RedisHandler();
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetSection("Appsettings:ConnectionString").Value);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            _authRepository = new AuthRepository(_transaction);
            _titleRepository = new TitleRepository(_transaction);

        }




        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                resetRepositories();
            }
        }
        private void resetRepositories()
        {
            _authRepository = null;
            _titleRepository = null;

        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {

                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }


                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
