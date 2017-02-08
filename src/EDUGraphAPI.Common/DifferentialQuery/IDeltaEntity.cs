/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDUGraphAPI.DifferentialQuery
{
    public interface IDeltaEntity
    {
        [JsonProperty("aad.isDeleted")]
        bool IsDeleted { get; set; }

        HashSet<string> ModifiedPropertyNames { get; }
    }
}