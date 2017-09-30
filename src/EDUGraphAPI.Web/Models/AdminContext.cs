/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Data;

namespace EDUGraphAPI.Web.Models
{
    public class AdminContext
    {
        public Organization Organization { get; private set; }

        public AdminContext(Organization organization)
        {
            this.Organization = organization;
        }

        public bool IsAdminConsented => Organization != null && Organization.IsAdminConsented;
    }
}