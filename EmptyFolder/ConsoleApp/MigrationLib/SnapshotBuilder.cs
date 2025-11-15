using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace MigrationLib
{
    /// <summary>
    /// Получение всех таблиц, столбцов, типов
    /// </summary>
    public class SnapshotBuilder
    {
        public Snapshot BuildFromAssembly(Assembly asm)
        {
            var snapshot = new Snapshot();

            var types = asm.GetTypes()
                .Where(t => t.GetCustomAttribute<TableAttribute>() != null);

            foreach (var type in types)
            {
                var tableAttr = type.GetCustomAttribute<TableAttribute>();
                if (tableAttr == null) continue;

                var table = new Table { Name = tableAttr.Name };

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    bool isPk = prop.GetCustomAttribute<PrimaryKeyAttribute>() != null;
                    bool isColumn = prop.GetCustomAttribute<ColumnAttribute>() != null || isPk;
                    if (!isColumn) continue;

                    var Type = prop.PropertyType;
                    if (Type != typeof(int) && Type != typeof(string))
                        throw new NotSupportedException("Поддерживаются только int и string");

                    table.Columns.Add(new Column
                    {
                        Name = prop.Name.ToLower(),
                        Type = Type == typeof(int) ? "int" : "string",
                        IsPrimaryKey = isPk
                    });
                }

                snapshot.Tables.Add(table);
            }

            return snapshot;
        }
    }
}
