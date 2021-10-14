using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoralTimeAdmin.Repositories
{
    /// <summary>
    /// General Interface for execution of CRUD commands
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDapperRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IList<T>> GetAllAsync();
        Task<int> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(int id);
    }

    /// <summary>
    /// General Interface for execution of stored procedures,sql commands
    /// </summary>
    public interface IDapperRepository
    {
        Task<IList<T>> ExecProc<T>(string storedProcedure, object parameters = null, int commandTimeout = 180);
        Task ExecProc(string storedProcedure, object parameters = null, int commandTimeout = 180);
        Task<IList<T>> ExecSql<T>(string sql, object parameters = null, int commandTimeout = 180);
        Task<T> ExecScalarSql<T>(string sql, object parameters = null, int commandTimeout = 180);
        Task<int> ExecNonQuery(string sql, object parameters = null, int commandTimeout = 180);
    }
}