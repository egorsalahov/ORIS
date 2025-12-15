using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary.Practice_7
{
    public class DatabaseExecutor<T> //делает полученный sql запрос в базу 
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

        private T Map(SqlDataReader reader) //Реализация Map, которой нет в конфлюенсе
        {
            var obj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var property = properties.FirstOrDefault(p =>
                    string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

                if (property != null && property.CanWrite)
                {
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                    if (value != null)
                    {
                        var propertyType = property.PropertyType;
                        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                        try
                        {
                            // Преобразуем значение к нужному типу
                            var convertedValue = Convert.ChangeType(value, underlyingType);
                            property.SetValue(obj, convertedValue);
                        }
                        catch (InvalidCastException)
                        {
                            // Если прямое преобразование не удалось, пробуем другие варианты
                            if (underlyingType == typeof(Guid) && value is string guidString)
                            {
                                property.SetValue(obj, Guid.Parse(guidString));
                            }
                            else if (underlyingType.IsEnum && value is int intValue)
                            {
                                property.SetValue(obj, Enum.ToObject(underlyingType, intValue));
                            }
                            else
                            {
                                // Для сложных случаев оставляем как есть
                                property.SetValue(obj, value);
                            }
                        }
                    }
                    else
                    {
                        // Для NULL значений устанавливаем default
                        property.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }
    }
}
