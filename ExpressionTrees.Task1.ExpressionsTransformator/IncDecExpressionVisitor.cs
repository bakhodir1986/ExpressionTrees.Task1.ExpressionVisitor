using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public class IncDecExpressionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<string, int> _keyValuePairs;

        public IncDecExpressionVisitor(Dictionary<string, int> keyValuePairs)
        {
            _keyValuePairs = keyValuePairs;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_keyValuePairs.ContainsKey(node.Name))
            {
                var value = _keyValuePairs[node.Name];

                return base.Visit(Expression.Constant(value));
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var result = Expression.Lambda(base.Visit(node.Body));

            return result;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression expression = null;

            if (node.Left is ConstantExpression a && (int)a.Value == 1 && node.NodeType is ExpressionType.Add)
            {
                expression = node.Right;
            }

            if (node.Right is ConstantExpression b && (int)b.Value == 1)
            {
                expression = node.Left;
            }

            if (expression == null)
            {
                return base.VisitBinary(node);
            }

            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Increment(base.Visit(expression));
                case ExpressionType.Subtract:
                    return Expression.Decrement(base.Visit(expression));
                default:
                    return base.VisitBinary(node);
            }
        }
    }
}
