using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationLib
{
    public interface ISqlGenerator
    {
        string MapClrTypeToSql(string clrType);
        string GenerateCreateTable(Table table);
        string GenerateDropTable(string tableName);
        string GenerateAddColumn(string tableName, Column column);
        string GenerateDropColumn(string tableName, string columnName);

        string GenerateAlterColumnType(string tableName, Column oldColumn, Column newColumn);
        string GenerateDropPrimaryKey(string tableName);
        string GenerateAddPrimaryKey(string tableName, string columnName);
    }

}
