using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary
{
    public class ExpressionParser
    {
        public string ParseExpression(Expression expression)
        {
            if (expression is BinaryExpression binary)
            {
                var left = ParseExpression(binary.Left);
                var right = ParseExpression(binary.Right);
                var op = GetSqlOperator(binary.NodeType);
                return $"({left} {op} {right})";
            }
            else if (expression is MemberExpression member)
            {
                return member.Member.Name;
            }
            else if (expression is ConstantExpression constant)
            {
                return FormatConstant(constant.Value);
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.GetType().Name}");
        }

        private string GetSqlOperator(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.LessThan => "<",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Unsupported node type: {nodeType}")
            };
        }

        private string FormatConstant(object value)
        {
            return value is string ? $"'{value}'" : value.ToString();
        }
    }
}
