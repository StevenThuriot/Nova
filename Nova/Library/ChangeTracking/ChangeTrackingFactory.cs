#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nova.Library.ChangeTracking
{
    /// <summary>
    /// Factory to create delegates to help with IChangeTracking.
    /// </summary>
    public static class ChangeTrackingFactory
    {
        private static readonly Dictionary<Type, object> Cache = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, ChangeTrackingHelper<object>> BoxedCache = new Dictionary<Type, ChangeTrackingHelper<object>>();
        
        private static readonly Dictionary<Type, object> ExtendedCache = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, ExtendedChangeTrackingHelper<object>> ExtendedBoxedCache = new Dictionary<Type, ExtendedChangeTrackingHelper<object>>();
        
        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void ClearCache()
        {
            Cache.Clear();
            BoxedCache.Clear();

            ExtendedCache.Clear();
            ExtendedBoxedCache.Clear();
        }



        /// <summary>
        /// Creates a helper containing a typed delegate that will check any of the properties that implement IChangeTracking if they have changed
        /// and a delegate that will accept changes for those properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static ChangeTrackingHelper<T> CreateHelper<T>(T item)
        {
            var type = item.GetType();
            var cache = Cache;

            object helper;           
            if (cache.TryGetValue(type, out helper))
                return (ChangeTrackingHelper<T>) helper;


            var properties = GetRelevantProperties(type);
            var hasChanges = HasChangesFor<T>(type, properties);
            var acceptChanges = AcceptChangesFor<T>(type, properties);

            var changeTrackingHelper = new ChangeTrackingHelper<T>(hasChanges, acceptChanges);
            cache.Add(type, changeTrackingHelper);

            return changeTrackingHelper;
        }

        /// <summary>
        /// Creates a boxed helper containing a delegate that will check any of the properties that implement IChangeTracking if they have changed
        /// and a delegate that will accept changes for those properties.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static ChangeTrackingHelper<object> CreateBoxedHelper(object item)
        {
            var type = item.GetType();
            var cache = BoxedCache;

            ChangeTrackingHelper<object> helper;
            if (cache.TryGetValue(type, out helper))
                return helper;
            
            var properties = GetRelevantProperties(type);
            var hasChanges = BoxedHasChangesFor(type, properties);
            var acceptChanges = BoxedAcceptChangesFor(type, properties);

            var changeTrackingHelper = new ChangeTrackingHelper<object>(hasChanges, acceptChanges);
            cache.Add(type, changeTrackingHelper);

            return changeTrackingHelper;
        }

        /// <summary>
        /// Creates an extended helper containing a delegate that will check any of the properties that implement IChangeTracking if they have changed,
        /// a delegate that will accept changes for those properties
        /// and a method call to make the properties stop listening for changes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static ExtendedChangeTrackingHelper<T> CreateExtendedHelper<T>(T item)
        {
            var type = item.GetType();
            var cache = ExtendedCache;

            object helper;
            if (cache.TryGetValue(type, out helper))
                return (ExtendedChangeTrackingHelper<T>) helper;

            var properties = GetRelevantProperties(type);
            var hasChanges = HasChangesFor<T>(type, properties);
            var acceptChanges = AcceptChangesFor<T>(type, properties);
            var stopChangeTracking = StopChangeTrackingFor<T>(type, properties);

            var changeTrackingHelper = new ExtendedChangeTrackingHelper<T>(hasChanges, acceptChanges, stopChangeTracking);
            cache.Add(type, changeTrackingHelper);

            return changeTrackingHelper;
        }

        /// <summary>
        /// Creates an extended boxed helper containing a delegate that will check any of the properties that implement IChangeTracking if they have changed,
        /// a delegate that will accept changes for those properties
        /// and a method call to make the properties stop listening for changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static ExtendedChangeTrackingHelper<object> CreateExtendedBoxedHelper(object item)
        {
            var type = item.GetType();
            var cache = ExtendedBoxedCache;

            ExtendedChangeTrackingHelper<object> helper;
            if (cache.TryGetValue(type, out helper))
                return helper;

            var properties = GetRelevantProperties(type);
            var hasChanges = BoxedHasChangesFor(type, properties);
            var acceptChanges = BoxedAcceptChangesFor(type, properties);
            var stopChangeTracking = BoxedStopChangeTrackingFor(type, properties);

            var changeTrackingHelper = new ExtendedChangeTrackingHelper<object>(hasChanges, acceptChanges, stopChangeTracking);
            cache.Add(type, changeTrackingHelper);

            return changeTrackingHelper;
        }









        private static Func<T, bool> HasChangesFor<T>(Type type, ICollection<PropertyInfo> properties)
        {   
            var instance = Expression.Parameter(type);
            
            if (properties.Count == 0)
            {
                return Compile<Func<T, bool>>(Expression.Constant(false), instance);
            }
            
            var hasChanges = CreateHasChangesExpression(properties, instance);
            return Compile<Func<T, bool>>(hasChanges, instance);
        }

        private static Action<T> AcceptChangesFor<T>(Type type, ICollection<PropertyInfo> properties)
        {
            var instance = Expression.Parameter(type);
            var acceptChanges = CreateAcceptChangesExpression(instance, properties);

            return Compile<Action<T>>(acceptChanges, instance);
        }

        private static Action<T> StopChangeTrackingFor<T>(Type type, ICollection<PropertyInfo> properties)
        {
            var instance = Expression.Parameter(type);
            var stopChangeTrackingExpression = CreateStopChangeTrackingExpression(instance, properties);

            return Compile<Action<T>>(stopChangeTrackingExpression, instance);
        }



        private static Func<object, bool> BoxedHasChangesFor(Type type, ICollection<PropertyInfo> properties)
        {
            var boxedInstance = Expression.Parameter(typeof(object));
            
            if (properties.Count == 0)
            {
                return Compile<Func<object, bool>>(Expression.Constant(false), boxedInstance);
            }

            var instance = Expression.Variable(type);
            var hasChanges = CreateHasChangesExpression(properties, instance);
            var block = UnboxAndExecute(boxedInstance, instance, hasChanges);
            
            return Compile<Func<object, bool>>(block, boxedInstance);
        }

        private static Action<object> BoxedAcceptChangesFor(Type type, ICollection<PropertyInfo> properties)
        {
            var boxedInstance = Expression.Parameter(typeof(object));

            var instance = Expression.Variable(type);
            var acceptChanges = CreateAcceptChangesExpression(instance, properties);

            var block = UnboxAndExecute(boxedInstance, instance, acceptChanges);
            
            return Compile<Action<object>>(block, boxedInstance);
        }

        private static Action<object> BoxedStopChangeTrackingFor(Type type, ICollection<PropertyInfo> properties)
        {
            var boxedInstance = Expression.Parameter(typeof(object));

            var instance = Expression.Variable(type);
            var acceptChanges = CreateStopChangeTrackingExpression(instance, properties);

            var block = UnboxAndExecute(boxedInstance, instance, acceptChanges);
            
            return Compile<Action<object>>(block, boxedInstance);
        }







        private static BlockExpression UnboxAndExecute(Expression boxedInstance, ParameterExpression instance, Expression expression)
        {
            var conversion = Expression.TypeAs(boxedInstance, instance.Type);
            var assignToInstance = Expression.Assign(instance, conversion);
            
            return Expression.Block(new[] { instance }, assignToInstance, expression);
        }
        
        private static ReadOnlyCollection<PropertyInfo> GetRelevantProperties(Type type)
        {
            var changeTrackingType = typeof (System.ComponentModel.IChangeTracking);
            var relevantProperties = type.GetProperties().Where(x => changeTrackingType.IsAssignableFrom(x.PropertyType));

            return relevantProperties.ToList().AsReadOnly();
        }

        private static T Compile<T>(Expression expression, ParameterExpression instance)
        {
            var lambda = Expression.Lambda<T>(expression, instance);
            var compiledLambda = lambda.Compile();

            return compiledLambda;
        }


        private static Expression CreateAcceptChangesExpression(Expression instance, ICollection<PropertyInfo> properties)
        {
            return CreateMethodCallerExpression(instance, properties, "AcceptChanges");
        }

        private static Expression CreateStopChangeTrackingExpression(Expression instance, ICollection<PropertyInfo> properties)
        {
            return CreateMethodCallerExpression(instance, properties, "StopChangeTracking");
        }

        private static Expression CreateMethodCallerExpression(Expression instance, ICollection<PropertyInfo> properties, string methodName)
        {
            if (properties.Count == 0)
                return Expression.Empty();

            var calls = new List<Expression>();

            foreach (var propertyInfo in properties)
            {
                var property = Expression.Property(instance, propertyInfo);

                var nullValue = Expression.Constant(null, property.Type);

                var value = Expression.Variable(property.Type);
                var assignment = Expression.Assign(value, property);

                var acceptChanges = Expression.Call(value, methodName, new Type[] {});

                var notNull = Expression.NotEqual(value, nullValue);
                var acceptIfNotNull = Expression.Condition(notNull, acceptChanges, Expression.Empty());

                var block = Expression.Block(new[] {value}, assignment, acceptIfNotNull);

                calls.Add(block);
            }

            return Expression.Block(calls);
        }

        private static Expression CreateHasChangesExpression(IEnumerable<PropertyInfo> properties, ParameterExpression instance)
        {
            Expression hasChanges = null;

            foreach (var propertyInfo in properties)
            {
                var property = Expression.Property(instance, propertyInfo);

                var nullValue = Expression.Constant(null, property.Type);
                var notNull = Expression.NotEqual(property, nullValue);
                var isChanged = Expression.Property(property, "IsChanged");
                var notNullAndHasChanges = Expression.AndAlso(notNull, isChanged);

                hasChanges = hasChanges == null
                                 ? notNullAndHasChanges
                                 : Expression.OrElse(hasChanges, notNullAndHasChanges);
            }

            return hasChanges;
        }
    }
}
