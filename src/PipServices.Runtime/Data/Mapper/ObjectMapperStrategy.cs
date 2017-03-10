﻿using System.Collections;
using System.Linq;
using System.Reflection;

namespace PipServices.Runtime.Data.Mapper
{
    internal sealed class ObjectMapperStrategy : IObjectMapperStrategy
    {
        public void Transfer<TS, TT>(IObjectMapper mapper, TS objectSource, TT objectTarget,
            PropertyInfo propertyInfoSource, PropertyInfo propertyInfoTarget)
            where TS : class
            where TT : class
        {
            var propertyValueSource = propertyInfoSource.GetValue(objectSource);
            var propertyValueTarget = propertyInfoTarget.GetValue(objectTarget);

            if (propertyValueSource == null)
                return;

            var valueSourceType = propertyValueSource.GetType();
            var valueSourceTypeInfo = valueSourceType.GetTypeInfo();

            var valueTargetType = propertyInfoTarget.PropertyType;
            var valueTargetTypeInfo = valueTargetType.GetTypeInfo();

            if (valueSourceTypeInfo.IsClass && valueSourceType != typeof(string) && !valueSourceTypeInfo.IsArray &&
                !valueSourceTypeInfo.ImplementedInterfaces.Contains(typeof(IEnumerable)))
            {
                var methodInfo = mapper.GetType().GetMethod(nameof(mapper.Transfer));
                var genericMethodInfo = methodInfo.MakeGenericMethod(valueSourceType, valueTargetType);
                var result = genericMethodInfo.Invoke(mapper, new[] {propertyValueSource});

                propertyInfoTarget.SetValue(objectTarget, result);

                return;
            }

            if (valueSourceType != typeof(string) &&
                (valueSourceTypeInfo.IsArray || valueSourceTypeInfo.ImplementedInterfaces.Contains(typeof(IEnumerable))))
            {
                var source = (IEnumerable) propertyValueSource;

                object firstEntry = null;
                foreach (var item in source)
                {
                    firstEntry = item;
                    break;
                }

                var entrySourceType = firstEntry?.GetType();

                if (entrySourceType != null)
                {
                    var entrySourceTypeInfo = entrySourceType.GetTypeInfo();
                    var entryTargetType = propertyValueTarget.GetType().GetGenericArguments()[0];

                    if (entrySourceTypeInfo.IsClass)
                    {
                        var methodParameters = new[]
                        {
                            entryTargetType
                        };

                        var method = propertyValueTarget.GetType().GetRuntimeMethod("Add", methodParameters);

                        foreach (var entrySource in source)
                        {
                            if (entrySource == null)
                                continue;

                            var methodInfo = mapper.GetType().GetMethod(nameof(mapper.Transfer));
                            var genericMethodInfo = methodInfo.MakeGenericMethod(entrySourceType, entryTargetType);
                            var entryTarget = genericMethodInfo.Invoke(mapper, new[] {entrySource});

                            var parameters = new[]
                            {
                                entryTarget
                            };

                            method.Invoke(propertyValueTarget, parameters);
                        }
                    }
                    else
                    {
                        var methodParameters = new[]
                        {
                            entrySourceType
                        };

                        var method = propertyValueTarget.GetType().GetRuntimeMethod("Add", methodParameters);

                        foreach (var entrySource in source)
                        {
                            var parameters = new[]
                            {
                                entrySource
                            };

                            method.Invoke(propertyValueTarget, parameters);
                        }
                    }

                    return;
                }
            }

            propertyInfoTarget.SetValue(objectTarget, propertyValueSource);
        }
    }
}