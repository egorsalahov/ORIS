using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary.Practice_7
{
    public class Repository<T> //торчит наружу чтобы просто вызывать методы 
    {
        private readonly SqlQueryBuilder<T> _queryBuilder = new SqlQueryBuilder<T>();
        private readonly DatabaseExecutor<T> _databaseExecutor;

        public Repository(SqlConnection connection)
        {
            _databaseExecutor = new DatabaseExecutor<T>(connection);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var sqlQuery = _queryBuilder.BuildSqlQuery(predicate, singleResult: true);
            return _databaseExecutor.ExecuteQuerySingle(sqlQuery);
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            var sqlQuery = _queryBuilder.BuildSqlQuery(predicate, singleResult: false);
            return _databaseExecutor.ExecuteQueryMultiple(sqlQuery);
        }
    }
}
