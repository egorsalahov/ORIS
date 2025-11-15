using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MigrationLib
{
    /// <summary>
    /// Генерирует миграцию
    /// </summary>
    public class MigrationGenerator
    {
        private readonly ISqlGenerator _sql;

        public MigrationGenerator(ISqlGenerator sql)
        {
            _sql = sql;
        }
        /// <summary>
        /// Метод распределяет, что ушло, что пришло и расскидывает это по переменным
        /// </summary>
        /// <param name="oldSnapshot">Версия без изменения</param>
        /// <param name="newSnapshot">Версия с изменением</param>
        /// <returns></returns>
        public (string upSql, string downSql) GenerateMigration(Snapshot oldSnapshot, Snapshot newSnapshot)
        {
            var up = new StringBuilder();
            var down = new StringBuilder();

            var oldTables = oldSnapshot.Tables.ToDictionary(t => t.Name);
            var newTables = newSnapshot.Tables.ToDictionary(t => t.Name);

            foreach (var newTable in newTables.Values)
            {
                if (!oldTables.ContainsKey(newTable.Name))
                {
                    up.AppendLine(_sql.GenerateCreateTable(newTable));
                    down.Insert(0, _sql.GenerateDropTable(newTable.Name) + "\n");
                }
            }

            foreach (var oldTable in oldTables.Values)
            {
                if (!newTables.ContainsKey(oldTable.Name))
                {
                    up.AppendLine(_sql.GenerateDropTable(oldTable.Name));
                    down.Insert(0, _sql.GenerateCreateTable(oldTable) + "\n");
                }
            }

            foreach (var newTable in newTables.Values)
            {
                if (!oldTables.TryGetValue(newTable.Name, out var oldTable))
                    continue;

                var oldCols = oldTable.Columns.ToDictionary(c => c.Name);
                var newCols = newTable.Columns.ToDictionary(c => c.Name);

                foreach (var col in newCols.Values)
                {
                    if (!oldCols.ContainsKey(col.Name))
                    {
                        up.AppendLine(_sql.GenerateAddColumn(newTable.Name, col));
                        down.Insert(0, _sql.GenerateDropColumn(newTable.Name, col.Name) + "\n");
                    }
                }

                foreach (var col in oldCols.Values)
                {
                    if (!newCols.ContainsKey(col.Name))
                    {
                        up.AppendLine(_sql.GenerateDropColumn(newTable.Name, col.Name));
                        down.Insert(0, _sql.GenerateAddColumn(newTable.Name, col) + "\n");
                    }
                }

                foreach (var newCol in newCols.Values)
                {
                    if (!oldCols.TryGetValue(newCol.Name, out var oldCol))
                        continue;

                    if (oldCol.Type != newCol.Type)
                    {
                        up.AppendLine(_sql.GenerateAlterColumnType(newTable.Name, oldCol, newCol));
                        down.Insert(0, _sql.GenerateAlterColumnType(newTable.Name, newCol, oldCol) + "\n");
                    }
                }

                string? oldPk = oldCols.Values.FirstOrDefault(c => c.IsPrimaryKey)?.Name;
                string? newPk = newCols.Values.FirstOrDefault(c => c.IsPrimaryKey)?.Name;

                if (oldPk != newPk)
                {
                    if (oldPk != null)
                    {
                        up.AppendLine(_sql.GenerateDropPrimaryKey(newTable.Name));
                    }
                    if (newPk != null)
                    {
                        up.AppendLine(_sql.GenerateAddPrimaryKey(newTable.Name, newPk));
                    }

                    if (newPk != null)
                    {
                        down.Insert(0, _sql.GenerateDropPrimaryKey(newTable.Name) + "\n");
                    }
                    if (oldPk != null)
                    {
                        down.Insert(0, _sql.GenerateAddPrimaryKey(newTable.Name, oldPk) + "\n");
                    }
                }
            }

            return (up.ToString(), down.ToString());
        }
    }
}
