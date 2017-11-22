﻿/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDUGraphAPI.DifferentialQuery
{
    public class DeltaRemovedData
    {
        [JsonProperty("reason")]
        string RemovedReason { get; set; }
    }
}