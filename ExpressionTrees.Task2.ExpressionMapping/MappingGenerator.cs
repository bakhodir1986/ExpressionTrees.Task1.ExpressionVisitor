using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var expressions = new List<Expression>();
            var sourceParam = Expression.Parameter(typeof(TSource));
            var destinationResult = Expression.New(typeof(TDestination));

            var sourceType = typeof(TSource);
            var sourceProperties = sourceType.GetProperties();

            var destinationType = typeof(TDestination);
            var destinationProperties = destinationType.GetProperties().ToDictionary(x => x.Name);

            var sourceInstance = Expression.Variable(typeof(TSource), "input");
            var destinationInstance = Expression.Variable(typeof(TDestination), "result");

            expressions.Add(Expression.Assign(sourceInstance, sourceParam));
            expressions.Add(Expression.Assign(destinationInstance, destinationResult));

            foreach (var sourceProperty in sourceProperties)
            {
                PropertyInfo destinationProperty = null;

                if (destinationProperties.TryGetValue(sourceProperty.Name, out destinationProperty))
                {
                    var sourceValue = Expression.Property(sourceInstance, sourceProperty.Name);
                    var destinationValue = Expression.Property(destinationInstance, destinationProperty);

                    expressions.Add(Expression.Assign(destinationValue, sourceValue));
                }
            }

            expressions.Add(destinationInstance);

            var body = Expression.Block(new[] { sourceInstance, destinationInstance }, expressions);

            var mapFunction =
                Expression.Lambda<Func<TSource, TDestination>>(
                    body,
                    sourceParam
                );

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }
    }
}
