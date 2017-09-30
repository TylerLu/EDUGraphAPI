/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using Newtonsoft.Json;

namespace EDUGraphAPI.DifferentialQuery
{
    public static class DeltaResultParser
    {
        // The implement of the method below is complex. Please focus on the result it returns.
        // To use this method, please make sure that the properties used to track changes of the generic type should be virtual.
        // Please check the User class under the DataSync folder.
        public static DeltaResult<Delta<TEntity>> Parse<TEntity>(string json) where TEntity : class
        {
            var deltaEntityType = DynamicProxyGenerator.CreateDeltaEntityProxyType<TEntity>();
            var type = typeof(DeltaResult<>).MakeGenericType(deltaEntityType);

            dynamic obj = JsonConvert.DeserializeObject(json, type);
            DeltaResult<TEntity> result = obj.Convert<TEntity>();

            return result.Convert(Delta.Create);
        }
    }
}