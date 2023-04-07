using AppNov14.SqlDataAccess;
using Microsoft.Extensions.Configuration;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AppNov14.Repositories
{
    public class BaseRepository
    {
        private readonly IConfiguration _configuration;

        private string _connectionString = null;

        protected BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection2");
        }     

        private static readonly TransactionOptions TransactionOptions = new TransactionOptions
        {
            IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            Timeout = TransactionManager.MaximumTimeout, // defined in machine.config, default value is 10 minutes
        };

        private static readonly TransactionOptions LongTransactionOptions = new TransactionOptions
        {
            IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            Timeout = TimeSpan.FromMinutes(30),
        };

        private BaseDataContext GetBaseDataContext()
        {
#if DEBUG
            var connection = new ProfiledDbConnection(new SqlConnection(this._connectionString), MiniProfiler.Current);
            var result = new BaseDataContext(connection);
#else
            var result = new BaseDataContext(this._connectionString);
#endif
            result.CommandTimeout = 0; // infinity

            return result;
        }

        protected BaseDataContext GetReadOnlyBaseDataContext()
        {
            var result = this.GetBaseDataContext();
            result.ObjectTrackingEnabled = false;

            return result;
        }

        protected TransactionScope GetReadOnlyTransactionScope(bool isLong = false)
        {
            return new TransactionScope(TransactionScopeOption.RequiresNew, (isLong ? BaseRepository.LongTransactionOptions : BaseRepository.TransactionOptions));
        }

        protected BaseDataContext GetBaseDataContextForDapper()
        {
            var result = this.GetBaseDataContext();
            result.ObjectTrackingEnabled = false;

            return result;
        }

        protected BaseDataContext GetWritableBaseDataContext()
        {
            var result = this.GetBaseDataContext();

            return result;
        }

        protected TransactionScope GetWritableTransactionScope(bool isLong = false)
        {
            return new TransactionScope(TransactionScopeOption.RequiresNew, (isLong ? BaseRepository.LongTransactionOptions : BaseRepository.TransactionOptions));
        }

        protected SqlConnection GetSqlConnection()
        {
            var result = new SqlConnection(this._connectionString);

            return result;
        }

        /// <summary>
        /// Выполняет запрос без возвращения результата
        /// </summary>
        /// <param name="commandText">Текст запроса</param>
        /// <param name="isStoredProcedure">True - хр. процедура</param>
        /// <param name="hasReturn">True - возвращает DataTable, иначе null</param>
        /// <param name="sqlParams">Параметры</param>
        protected DataTable ExecuteQuery(string commandText, bool isStoredProcedure = false, bool hasReturn = false, params SqlParameter[] sqlParams)
        {
            DataTable result = null;

            using (var connection = GetSqlConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 0; // infinity
                    if (isStoredProcedure)
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                    }

                    foreach (var param in sqlParams)
                    {
                        if (param.Value == null)
                            param.Value = DBNull.Value;
                        command.Parameters.Add(param);
                    }

                    if (!hasReturn)
                    {
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            result = new DataTable();
                            adapter.Fill(result);
                        }
                    }
                }

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Выполняет хр. процедуру без результата
        /// </summary>
        /// <param name="storeProcedureName">Имя хр. процедуры</param>
        /// <param name="hasReturn">True - возвращает DataTable, иначе null</param>
        /// <param name="sqlParams">Параметры</param>
        protected DataTable ExecuteProcedure(string storeProcedureName, bool hasReturn, params SqlParameter[] sqlParams)
        {
            return this.ExecuteQuery(storeProcedureName, true, hasReturn, sqlParams);
        }
    }
}
