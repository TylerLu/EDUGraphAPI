/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EDUGraphAPI.Web.Infrastructure
{
    /// <summary>
    /// This SimpleMapper is inefficient, and should not be used in production environment.
    /// </summary>
    public static class SimpleMapper
    {
        /// <summary>
        /// Execute a mapping from the source object to the existing target object. 
        /// </summary>
        public static void Map<TSource, TTarget>(TSource source, TTarget target, IEnumerable<string> properties)
        {
            var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var targetProperties = typeof(TTarget).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(i => i.Name == property);
                if (sourceProperty == null || !sourceProperty.GetMethod.IsPublic) continue;

                var targetProperty = targetProperties.FirstOrDefault(i => i.Name == property);
                if (targetProperty == null || !targetProperty.SetMethod.IsPublic) continue;

                var value = sourceProperty.GetValue(source);
                targetProperty.SetValue(target, value);
            }
        }
    }
}