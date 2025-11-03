using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary.Practice_7
{
    public class DatabaseExecutor<T>
    {
        private readonly SqlConnection _connection;

        public DatabaseExecutor(SqlConnection connection)
        {
            _connection = connection;
        }

        public T ExecuteQuerySingle(string query)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = query;
                _connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Map(reader);
                    }
                }
                _connection.Close();
            }

            return default(T);
        }

        public IEnumerable<T> ExecuteQueryMultiple(string query)
        {
            var results = new List<T>();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = query;
                _connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(Map(reader));
                    }
                }
                _connection.Close();
            }
            return results;
        }

        private T Map(SqlDataReader reader)
        {
            // Реализация маппинга из reader в объект T
            throw new NotImplementedException();
        }
    }
}
