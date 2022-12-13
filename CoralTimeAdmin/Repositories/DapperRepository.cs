using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CoralTimeAdmin.Controllers;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CoralTimeAdmin.Repositories
{
    public class DapperRepository : IDapperRepository
    {
        private readonly String _connectionString;
        private readonly ILogger<DapperRepository> _logger;

        public DapperRepository(ILogger<DapperRepository> logger) {
            _connectionString = ConfigurationManager.ConnectionStrings["CoralTimeContext"].ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// executes the stored procedure and returns a result set.Parameters can be  input,output 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<IList<T>> ExecProc<T>(string storedProcedure, object parameters = null, int commandTimeout = 180) {
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    if (parameters != null) {
                        return (await connection.QueryAsync<T>(storedProcedure, parameters,
                            commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout)).ToList();
                    }
                    else {
                        return (await connection.QueryAsync<T>(storedProcedure,
                            commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout)).ToList();
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(444, ex, "ExecProc<T>: {Procedure}", storedProcedure);
                return await Task.FromResult(new List<T>());
            }
        }


        /// <summary>
        /// executes the stored procedure.Parameters can be input,output
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task ExecProc(string storedProcedure, object parameters = null, int commandTimeout = 180) {

            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    if (parameters != null) {
                        await connection.QueryAsync(storedProcedure, parameters,
                           commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    }
                    else {
                        await connection.QueryAsync(storedProcedure,
                            commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(444, ex, "ExecProc: {Procedure}", storedProcedure);
            }
        }

        /// <summary>
        /// executes the sql command and returns a result set.Parameters can be  only input 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<IList<T>> ExecSql<T>(string sql, object parameters = null, int commandTimeout = 180) {
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    if (parameters != null) {
                        return (await connection.QueryAsync<T>(sql, parameters,
                           commandType: CommandType.Text, commandTimeout: commandTimeout)).ToList();
                    }
                    else {
                        return (await connection.QueryAsync<T>(sql,
                            commandType: CommandType.Text, commandTimeout: commandTimeout)).ToList();
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(444, ex, "sql");
                return await Task.FromResult(new List<T>());
            }
        }

        /// <summary>
        /// executes the sql command and returns a simple value.Parameters can be  only input 
        /// This method is valid only for execution of scalar sqls
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> ExecScalarSql<T>(string sql, object parameters = null, int commandTimeout = 180) {
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    if (parameters != null) {
                        return await connection.ExecuteScalarAsync<T>(sql, parameters,
                           commandType: CommandType.Text, commandTimeout: commandTimeout);
                    }
                    else {
                        return await connection.ExecuteScalarAsync<T>(sql,
                            commandType: CommandType.Text, commandTimeout: commandTimeout);
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(444, ex, "ExecScalarSql<T>");
                return await Task.FromResult(default(T));
            }
        }

        /// <summary>
        /// executes the sql command and returns the number of affected rows.Parameters can be  only input 
        /// This method is valid only for CRUD sqls
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<int> ExecNonQuery(string sql, object parameters = null, int commandTimeout = 180) {
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    var affectedRows = await connection.ExecuteAsync(sql, parameters, null, commandTimeout);
                    return affectedRows;
                }
            }
            catch (Exception ex) {
                _logger.LogError(444, ex, "ExecNonQuery");
                return await Task.FromResult(0);
            }
        }
        
    }
}