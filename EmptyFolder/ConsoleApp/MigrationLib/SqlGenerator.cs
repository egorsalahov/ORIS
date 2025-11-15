using System;
using System.Collections.Generic;
using System.Text;

namespace MigrationLib
{

    /// <summary>
    /// Реализация генератора SQL для PostgreSQL (CREATE TABLE, ALTER TABLE и т.п.).
    /// </summary>
    public class PostgresSqlGenerator : ISqlGenerator
    {
        public string MapClrTypeToSql(string clrType) => clrType switch
        {
            "int" => "INTEGER",
            "string" => "TEXT",
            _ => throw new NotSupportedException()
        };

        public string GenerateCreateTable(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {table.Name} (");

            var defs = new List<string>();
            foreach (var col in table.Columns)
            {
                var sqlType = MapClrTypeToSql(col.Type);
                var def = $"{col.Name} {sqlType}";
                if (col.IsPrimaryKey)
                    def += " PRIMARY KEY";
                defs.Add(def);
            }

            sb.AppendLine(string.Join(",\n", defs));
            sb.AppendLine(");");
            return sb.ToString();
        }

        public string GenerateDropTable(string tableName)
            => $"DROP TABLE {tableName};";

        public string GenerateAddColumn(string tableName, Column column)
            => $"ALTER TABLE {tableName} ADD COLUMN {column.Name} {MapClrTypeToSql(column.Type)};";

        public string GenerateDropColumn(string tableName, string columnName)
            => $"ALTER TABLE {tableName} DROP COLUMN {columnName};";

        public string GenerateAlterColumnType(string tableName, Column oldColumn, Column newColumn)
        {
            var sqlType = MapClrTypeToSql(newColumn.Type);

            return
                $"ALTER TABLE {tableName} ALTER COLUMN {newColumn.Name} TYPE {sqlType} USING {newColumn.Name}::{sqlType};";
        }

        public string GenerateDropPrimaryKey(string tableName)
        {

            return $"ALTER TABLE {tableName} DROP CONSTRAINT {tableName}_pkey;";
        }

        public string GenerateAddPrimaryKey(string tableName, string columnName)
        {
            return $"ALTER TABLE {tableName} ADD PRIMARY KEY ({columnName});";
        }
    }
}
