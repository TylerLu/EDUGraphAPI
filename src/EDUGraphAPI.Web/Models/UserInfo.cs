/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
namespace EDUGraphAPI.Web.Models
{
    public class UserInfo
    {
        public string Id { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string Mail { get; set; }

        public string UserPrincipalName { get; set; }

        public string[] Roles { get; set; }
    }
}