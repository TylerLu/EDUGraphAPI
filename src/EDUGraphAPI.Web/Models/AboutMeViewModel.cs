/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System.Collections.Generic;

namespace EDUGraphAPI.Web.Models
{
    public class AboutMeViewModel
    {
        public AboutMeViewModel()
        {
            this.Groups = new List<string>();
        }

        public string Username { get; set; }

        public string MyFavoriteColor { get; set; }

        public List<ColorEntity> FavoriteColors { get; set; }

        public List<string> Groups { get; set; }

        public bool ShowFavoriteColor { get; set; }
    }
}