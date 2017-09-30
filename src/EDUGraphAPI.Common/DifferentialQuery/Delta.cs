/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System.Collections.Generic;

namespace EDUGraphAPI.DifferentialQuery
{
    public class Delta<TEntity> where TEntity : class
    {
        public Delta(TEntity entity)
        {
            this.Entity = entity;
        }

        public TEntity Entity { get; private set; }

        public bool IsDeleted => (Entity as IDeltaEntity).IsDeleted;

        public HashSet<string> ModifiedPropertyNames => (Entity as IDeltaEntity).ModifiedPropertyNames;
    }

    public class Delta
    {
        public static Delta<TEntity> Create<TEntity>(TEntity entity) where TEntity : class
        {
            return new Delta<TEntity>(entity);
        }
    }
}