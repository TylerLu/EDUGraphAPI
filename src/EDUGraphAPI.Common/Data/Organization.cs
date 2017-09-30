/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System;

namespace EDUGraphAPI.Data
{
    /// <summary>
    /// An instance of the class represents a tenant in Azure AD
    /// </summary>
    public class Organization
    {
        public int Id { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public bool IsAdminConsented { get; set; }

        public string Issuer => $"https://sts.windows.net/{TenantId}/";
    }
}