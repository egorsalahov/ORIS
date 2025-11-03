using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary.Practice_7
{
    public class SqlQueryBuilder<T>
    {
        private readonly ExpressionParser _expressionParser = new ExpressionParser();

        public string BuildSqlQuery(Expression<Func<T, bool>> predicate, bool singleResult)
        {
            var tableName = typeof(T).Name + "s";
            var whereClause = _expressionParser.ParseExpression(predicate.Body);
            var limitClause = singleResult ? "LIMIT 1" : string.Empty;

            return $"SELECT * FROM {tableName} WHERE {whereClause} {limitClause}".Trim();
        }
    }
}
